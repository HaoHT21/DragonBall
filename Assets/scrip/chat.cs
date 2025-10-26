using UnityEngine;
using TMPro;
using System.Collections;

// Class định nghĩa nội dung cho mỗi chu kỳ hội thoại
[System.Serializable]
public class CycleContent
{
    [Header("Nội dung cho Panel 1")]
    [TextArea(3, 5)] public string contentForPanel1;

    [Header("Nội dung cho Panel 2")]
    [TextArea(3, 5)] public string contentForPanel2;
}

public class chat : MonoBehaviour
{
    [Header("Panel 1 (Panel A)")]
    public GameObject panel1Object;
    public TextMeshProUGUI panel1Text;

    [Header("Panel 2 (Panel B)")]
    public GameObject panel2Object;
    public TextMeshProUGUI panel2Text;

    [Header("Dữ liệu Kịch bản")]
    public CycleContent[] fullScript;

    private NPCInteraction currentNPC;
    private int currentCycleIndex = 0;
    private bool isPanel1Active = true;
    public float typingSpeed = 0.05f;

    void Start()
    {
        if (panel2Object != null) panel2Object.SetActive(false);
        if (panel1Object != null) panel1Object.SetActive(false);
    }

    // Bắt đầu chuỗi hội thoại
    public void StartCycling(CycleContent[] script, NPCInteraction npc)
    {
        if (script == null || script.Length == 0) return;

        Time.timeScale = 0f;
        currentNPC = npc;

        fullScript = script;
        currentCycleIndex = 0;
        isPanel1Active = true;

        panel2Object.SetActive(false);
        panel1Object.SetActive(true);
        StartCoroutine(TypeContent(panel1Text, fullScript[currentCycleIndex].contentForPanel1));
    }

    void Update()
    {
        if (currentNPC == null || fullScript == null || fullScript.Length == 0 ||
            (!panel1Object.activeSelf && !panel2Object.activeSelf))
            return;

        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter) ||
            Input.GetMouseButtonDown(0))
        {
            HandleCycleProgression();
        }
    }

    private void HandleCycleProgression()
    {
        TextMeshProUGUI activeText = isPanel1Active ? panel1Text : panel2Text;
        string expectedContent = isPanel1Active ? fullScript[currentCycleIndex].contentForPanel1 :
                                                  fullScript[currentCycleIndex].contentForPanel2;

        // Nếu hiệu ứng gõ chữ chưa xong → hoàn thành ngay
        if (activeText.text != expectedContent)
        {
            StopAllCoroutines();
            activeText.text = expectedContent;
            return;
        }

        // Nếu đang ở Panel 1 → chuyển sang Panel 2
        if (isPanel1Active)
        {
            panel1Object.SetActive(false);
            panel2Object.SetActive(true);
            isPanel1Active = false;
            StartCoroutine(TypeContent(panel2Text, fullScript[currentCycleIndex].contentForPanel2));
        }
        else
        {
            // Đã xong 1 cặp hội thoại → chuyển sang chu kỳ kế tiếp
            currentCycleIndex++;

            if (currentCycleIndex < fullScript.Length)
            {
                panel2Object.SetActive(false);
                panel1Object.SetActive(true);
                isPanel1Active = true;
                StartCoroutine(TypeContent(panel1Text, fullScript[currentCycleIndex].contentForPanel1));
            }
            else
            {
                // Hết kịch bản
                EndCycling();
            }
        }
    }

    IEnumerator TypeContent(TextMeshProUGUI textComponent, string content)
    {
        textComponent.text = "";
        foreach (char c in content.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
    }

    void EndCycling()
    {
        panel1Object.SetActive(false);
        panel2Object.SetActive(false);

        // Khôi phục game
        Time.timeScale = 1f;

        // Gọi hàm bắt đầu chiến đấu hoặc thay đổi trạng thái NPC
        if (currentNPC != null)
        {
            currentNPC.BeginCombat();
            currentNPC = null;
        }

        // === PHÂN BIỆT LOẠI MỤC TIÊU ===
        if (SceneGoalManager.Instance != null)
        {
            var manager = SceneGoalManager.Instance;

            // 🧠 Chỉ mở cổng nếu mục tiêu hiện tại là loại NPCInteraction
            if (manager.goalType == SceneGoalManager.GoalType.NPCInteraction)
            {
                manager.OnNPCInteractionComplete();
            }
            else
            {
                Debug.Log("[CHAT] Hội thoại kết thúc nhưng GoalType hiện tại không phải NPCInteraction → không mở cổng.");
            }
        }
        else
        {
            Debug.LogError("[CRITICAL ERROR CHAT] Không thể tìm thấy SceneGoalManager Instance. Cổng sẽ không hiện ra sau đối thoại.");
        }

        Debug.Log("Kết thúc chuỗi hội thoại. Chuỗi hội thoại đã hoàn tất.");
    }
}
