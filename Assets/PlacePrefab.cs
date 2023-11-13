using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation.Samples;
using UnityEngine.SceneManagement;

public class PlacePrefab : MonoBehaviour
{
    [SerializeField] private GameObject letterPrefab;
    [SerializeField] private GameObject toyPrefab;
    [SerializeField] private Animator ToyAnim;
    [SerializeField] private Transform userDirection;
    [SerializeField] private GameObject dialogBox;
    [SerializeField] private GameObject reloadBtn;
    [SerializeField] private GameObject particleDance;
    [SerializeField] private TextMeshProUGUI countTMP;
    [SerializeField] private TextMeshProUGUI tmpText;
    [SerializeField] private List<AudioSource> audioSources;
    [SerializeField] private List<AudioClip> audioClips;
    [SerializeField] private List<GameObject> emojiParticle;
    [SerializeField] private GameObject particleEmojiContainer;
    [SerializeField] private AudioClip audioEffect;
    [SerializeField] private GameObject TalkingObj;
    private List<string> dialogLines = new List<string>()
    {
        "Marathon Education Teachers, Happy Vietnamese Teachers' Day! Your dedication and enthusiasm for teaching illuminate every student's road to knowledge. Thank you for making learning an exciting journey.",
        "Happy Teacher's Day to all of the Marathon Education mentors! Your guidance develops minds and fosters a love of learning. I hope your influence only gets stronger.",
        "Warm wishes to the entire teaching team on this special day. Your commitment to education creates a brighter future for every student you touch.",
        "Let's honor the steadfast commitment of Marathon Education teachers on Vietnamese Teachers' Day. Your impact goes far beyond the classroom.",
        "Thank you, Marathon Education teachers in specifically and to all the teacher in Vietnam in general, for the positive difference you make every day. Wishing you a day filled with appreciation and gratitude. Happy Teachers' Day!",
        "Best wishes, a dance from Bill - Codi Team With Love! ;) <3 click to watch this show!"

    }; // Danh sách các dòng đối thoại
    [SerializeField] private PlaceOnPlane placeOnPlane;

    private float scaleSpeed = 2f;
    private float minScale = 0.07f;
    private float maxScale = 0.1f;
    private bool texting = false;

    private int currentLine = 0;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RotateIndicator();
        RotateTowardsTarget();
    }
    private void RotateIndicator()
    {
        transform.GetChild(0).Rotate(Vector3.up, 70 * Time.deltaTime);
        float scaleValue = Mathf.Lerp(minScale, maxScale, Mathf.SmoothStep(0f, 1f, Mathf.PingPong(Time.time * scaleSpeed, 1f)));
        transform.GetChild(0).localScale = new Vector3(scaleValue, scaleValue, scaleValue);
    }
    void RotateTowardsTarget()
    {
        if (userDirection == null)
        {
            Debug.LogWarning("Target Transform is not set!");
            return;
        }
        Vector3 directionToTarget = userDirection.position - toyPrefab.transform.position;
        directionToTarget.y = 0f;
        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            toyPrefab.transform.rotation = Quaternion.Slerp(toyPrefab.transform.rotation, targetRotation, 50 * Time.deltaTime);
        }
    }
    public void ShowLetter()
    {
        if (!gameObject.activeInHierarchy)
            return;
        letterPrefab.SetActive(true);
    }
    public void AppearToy()
    {
        toyPrefab.SetActive(true);
        placeOnPlane.ready = true;
        StartCoroutine(Speak());
    }
    public IEnumerator Speak()
    {
        yield return new WaitForSeconds(1);
        ShowDialog();
    }
    private void ShowDialog()
    {
        dialogBox.SetActive(true);
    }

    public void ShowNextDialog()
    {
        if (texting)
            return;
        // Kiểm tra xem có thêm dòng đối thoại không
        if (currentLine < dialogLines.Count)
        {
            // Chuyển sang dòng tiếp theo
            if (currentLine == 0)
                ToyAnim.SetTrigger("idleB");
            audioSources[2].PlayOneShot(audioClips[currentLine]);
            StartCoroutine(ShowText());
        }
        else
        {
            // Ẩn hộp thoại khi đã hiển thị hết đối thoại
            dialogBox.SetActive(false);
            StartCoroutine(CountDance());
        }
    }
    private IEnumerator ShowText()
    {
        texting = true;
        string text = "";
        tmpText.text = "";
        int randNum = Random.Range(0, emojiParticle.Count);
        GameObject pickParticle = Instantiate(emojiParticle[randNum], particleEmojiContainer.transform);
        Debug.Log(emojiParticle[randNum]);
        foreach (char character in dialogLines[currentLine])
        {
            // Thực hiện các hành động với từng ký tự
            Debug.Log(character);
            text += character;
            tmpText.text = $"{text}_";
            yield return new WaitForSeconds(0.05f);
        }
        Destroy(pickParticle);
        text.Substring(0, text.Length - 1);
        tmpText.text = text;
        texting = false;
        currentLine++;
    }
    public IEnumerator ReloadTrigger()
    {
        yield return new WaitUntil(() => !audioSources[1].isPlaying);
        audioSources[0].Play();
        yield return new WaitForSeconds(2);
        reloadBtn.SetActive(true);
    }
    private IEnumerator CountDance()
    {
        countTMP.gameObject.SetActive(true);
        ToyAnim.SetTrigger("rally");
        audioSources[0].Stop();
        audioSources[1].Play();
        TalkingObj.SetActive(false);
        for (int i = 1; i <= 3; i++)
        {
            countTMP.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        countTMP.gameObject.SetActive(false);

        Dance();
    }
    public void Dance()
    {
        audioSources[2].Stop();
        audioSources[2].PlayOneShot(audioEffect);
        ToyAnim.SetTrigger("dance");
        particleDance.SetActive(true);
        StartCoroutine(ReloadTrigger());
    }
    public void ReloadScene()
    {
        // Lấy index của scene hiện tại
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Tải lại scene hiện tại bằng cách sử dụng index
        SceneManager.LoadScene(currentSceneIndex);
    }
}
