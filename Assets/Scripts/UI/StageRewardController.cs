using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace NovelianMagicLibraryDefense.UI
{
    /// <summary>
    /// Controls stage reward display based on grade (F, B, A, S)
    /// Updates slider value according to the achieved grade
    /// </summary>
    public class StageRewardController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Slider rewardSlider;
        [SerializeField] private TextMeshProUGUI gradeText;

        [Header("Grade Buttons")]
        [SerializeField] private Button gradeF_Button;
        [SerializeField] private Button gradeB_Button;
        [SerializeField] private Button gradeA_Button;
        [SerializeField] private Button gradeS_Button;

        [Header("Grade Settings")]
        [SerializeField] private float gradeF_Value = 25f;
        [SerializeField] private float gradeB_Value = 50f;
        [SerializeField] private float gradeA_Value = 75f;
        [SerializeField] private float gradeS_Value = 100f;

        private void Awake()
        {
            SetupGradeButtons();

            // Initialize slider
            if (rewardSlider != null)
            {
                rewardSlider.minValue = 0f;
                rewardSlider.maxValue = 100f;
                rewardSlider.value = 0f;
            }
        }

        private void SetupGradeButtons()
        {
            if (gradeF_Button != null)
            {
                gradeF_Button.onClick.AddListener(() => SetGrade(StageGrade.F));
            }

            if (gradeB_Button != null)
            {
                gradeB_Button.onClick.AddListener(() => SetGrade(StageGrade.B));
            }

            if (gradeA_Button != null)
            {
                gradeA_Button.onClick.AddListener(() => SetGrade(StageGrade.A));
            }

            if (gradeS_Button != null)
            {
                gradeS_Button.onClick.AddListener(() => SetGrade(StageGrade.S));
            }
        }

        private void OnDestroy()
        {
            if (gradeF_Button != null) gradeF_Button.onClick.RemoveAllListeners();
            if (gradeB_Button != null) gradeB_Button.onClick.RemoveAllListeners();
            if (gradeA_Button != null) gradeA_Button.onClick.RemoveAllListeners();
            if (gradeS_Button != null) gradeS_Button.onClick.RemoveAllListeners();
        }

        /// <summary>
        /// Set stage grade and update slider value
        /// </summary>
        public void SetGrade(StageGrade grade)
        {
            float targetValue = GetGradeValue(grade);

            if (rewardSlider != null)
            {
                rewardSlider.value = targetValue;
            }

            if (gradeText != null)
            {
                gradeText.text = grade.ToString();
            }

            Debug.Log($"[StageRewardController] Grade set to {grade}, slider value: {targetValue}");
        }

        /// <summary>
        /// Get slider value for specific grade
        /// </summary>
        private float GetGradeValue(StageGrade grade)
        {
            return grade switch
            {
                StageGrade.F => gradeF_Value,
                StageGrade.B => gradeB_Value,
                StageGrade.A => gradeA_Value,
                StageGrade.S => gradeS_Value,
                _ => 0f
            };
        }

        /// <summary>
        /// Set grade by string (useful for external calls)
        /// </summary>
        public void SetGradeByString(string gradeString)
        {
            if (System.Enum.TryParse<StageGrade>(gradeString, out StageGrade grade))
            {
                SetGrade(grade);
            }
            else
            {
                Debug.LogWarning($"[StageRewardController] Invalid grade string: {gradeString}");
            }
        }

        /// <summary>
        /// Calculate grade based on score (0-100)
        /// </summary>
        public void SetGradeByScore(float score)
        {
            StageGrade grade;

            if (score >= 90f)
                grade = StageGrade.S;
            else if (score >= 75f)
                grade = StageGrade.A;
            else if (score >= 50f)
                grade = StageGrade.B;
            else
                grade = StageGrade.F;

            SetGrade(grade);
        }
    }

    /// <summary>
    /// Stage grade enum
    /// </summary>
    public enum StageGrade
    {
        F,  // 25
        B,  // 50
        A,  // 75
        S   // 100
    }
}
