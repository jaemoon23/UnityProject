using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace Dispatch
{
    /// <summary>
    /// 파견 시스템 테스트 UI 패널
    /// CSV 데이터 기반 보상 시스템
    /// </summary>
    public class DispatchTestPanel : MonoBehaviour
    {
        [Header("파견 매니저 참조")]
        [SerializeField] private DispatchManager dispatchManager;

        [Header("UI 요소")]
        [SerializeField] private Slider timeSlider;                      // 시간 선택 슬라이더
        [SerializeField] private TextMeshProUGUI selectedTimeText;       // 선택된 시간 표시
        [SerializeField] private TextMeshProUGUI descriptionText;        // 파견 설명
        [SerializeField] private TextMeshProUGUI rewardInfoText;         // 보상 정보 표시
        [SerializeField] private Button dispatchButton;                  // 파견하기 버튼

        [Header("테스트 설정")]
        [SerializeField] private DispatchLocation testLocation = DispatchLocation.MagicResiduePurification;  // 테스트용 파견 장소

        private int currentSelectedHours = 4;
        private int currentSelectedTimeID;
        private List<DispatchTimeTableData> availableTimes;

        private void Start()
        {
            LoadCSVData();
            InitializeUI();
            SetupEventListeners();
        }

        /// <summary>
        /// CSV 데이터 로드
        /// </summary>
        private void LoadCSVData()
        {
            // CSV 로더가 초기화될 때까지 대기
            if (!CSVLoader.Instance.IsInit)
            {
                Debug.LogWarning("[DispatchTestPanel] CSVLoader가 아직 초기화되지 않았습니다. 잠시 후 다시 시도하세요.");
                return;
            }

            // 파견 시간 테이블 로드
            var timeTable = CSVLoader.Instance.GetTable<DispatchTimeTableData>();
            if (timeTable != null)
            {
                availableTimes = timeTable.FindAll(x => true).OrderBy(x => x.Required_Hours).ToList();
                Debug.Log($"[DispatchTestPanel] 파견 시간 데이터 로드 완료: {availableTimes.Count}개");
            }
            else
            {
                Debug.LogError("[DispatchTestPanel] 파견 시간 테이블을 로드할 수 없습니다!");
            }
        }

        /// <summary>
        /// UI 초기화
        /// </summary>
        private void InitializeUI()
        {
            if (availableTimes == null || availableTimes.Count == 0)
            {
                Debug.LogError("[DispatchTestPanel] 파견 시간 데이터가 없습니다!");
                return;
            }

            // 슬라이더 설정 (0 ~ 시간 옵션 개수 - 1)
            timeSlider.minValue = 0;
            timeSlider.maxValue = availableTimes.Count - 1;
            timeSlider.wholeNumbers = true;
            timeSlider.value = 0;

            UpdateTimeDisplay(0);
        }

        /// <summary>
        /// 이벤트 리스너 설정
        /// </summary>
        private void SetupEventListeners()
        {
            timeSlider.onValueChanged.AddListener(OnTimeSliderChanged);
            dispatchButton.onClick.AddListener(OnDispatchButtonClicked);
        }

        /// <summary>
        /// 슬라이더 값 변경 시
        /// </summary>
        private void OnTimeSliderChanged(float value)
        {
            int index = Mathf.RoundToInt(value);
            UpdateTimeDisplay(index);
        }

        /// <summary>
        /// 시간 표시 업데이트 및 보상 정보 표시
        /// </summary>
        private void UpdateTimeDisplay(int index)
        {
            if (availableTimes == null || index >= availableTimes.Count)
                return;

            var timeData = availableTimes[index];
            currentSelectedHours = (int)timeData.Required_Hours;
            currentSelectedTimeID = timeData.Dispatch_Time_ID;

            // 선택된 시간 텍스트
            selectedTimeText.text = $"{currentSelectedHours}시간";

            // 파견 장소 정보 가져오기
            var locationData = GetLocationData(testLocation);
            if (locationData == null)
            {
                descriptionText.text = "장소 정보를 찾을 수 없습니다.";
                return;
            }

            // 보상 정보 가져오기
            var rewardData = GetRewardData(locationData.Dispatch_Location_ID, currentSelectedTimeID);
            if (rewardData == null)
            {
                descriptionText.text = $"{currentSelectedHours}시간 파견\n보상 정보를 찾을 수 없습니다.";
                return;
            }

            // 설명 텍스트
            descriptionText.text = $"<size=auto><b>{GetLocationName(testLocation)}</b></size>\n" +
                                   $"파견 시간: {currentSelectedHours}시간\n" +
                                   $"<color=yellow>보상 배율: x{rewardData.Reward_Multiplier}</color>";

            // 보상 상세 정보 표시
            DisplayRewardInfo(rewardData);
        }

        /// <summary>
        /// 보상 정보 표시
        /// </summary>
        private void DisplayRewardInfo(DispatchRewardTableData rewardData)
        {
            if (rewardInfoText == null) return;

            // 보상 그룹 데이터 가져오기
            var rewardGroupData = CSVLoader.Instance.GetData<RewardGroupData>(rewardData.Reward_Group_ID);
            if (rewardGroupData == null)
            {
                rewardInfoText.text = "보상 그룹 정보 없음";
                return;
            }

            // 보상 아이템 목록 가져오기
            List<string> rewardTexts = new List<string>();

            // Reward_1_ID ~ Reward_5_ID 체크
            int[] rewardIDs = new int[]
            {
                rewardGroupData.Reward_1_ID,
                rewardGroupData.Reward_2_ID,
                rewardGroupData.Reward_3_ID,
                rewardGroupData.Reward_4_ID,
                rewardGroupData.Reward_5_ID
            };

            foreach (var rewardID in rewardIDs)
            {
                if (rewardID == 0) continue; // 0이면 보상 없음

                var reward = CSVLoader.Instance.GetData<RewardData>(rewardID);
                if (reward != null)
                {
                    int minCount = Mathf.FloorToInt(reward.Min_Count * rewardData.Reward_Multiplier);
                    int maxCount = Mathf.FloorToInt(reward.Max_Count * rewardData.Reward_Multiplier);

                    string fixedText = reward.Is_Fixed ? "[고정]" : $"[{reward.Probability * 100:F0}%]";
                    rewardTexts.Add($"{fixedText} 아이템 ID {reward.Item_ID}: {minCount}~{maxCount}개");
                }
            }

            if (rewardTexts.Count > 0)
            {
                rewardInfoText.text = "<b>예상 보상:</b>\n" + string.Join("\n", rewardTexts);
            }
            else
            {
                rewardInfoText.text = "보상 정보 없음";
            }
        }

        /// <summary>
        /// 파견 장소 데이터 가져오기
        /// </summary>
        private DispatchLocationData GetLocationData(DispatchLocation location)
        {
            var locationTable = CSVLoader.Instance.GetTable<DispatchLocationData>();
            if (locationTable == null) return null;

            return locationTable.FindAll(x => x.Dispatch_Location == location).FirstOrDefault();
        }

        /// <summary>
        /// 파견 장소 이름 가져오기
        /// </summary>
        private string GetLocationName(DispatchLocation location)
        {
            switch (location)
            {
                case DispatchLocation.NightmareWarehouse: return "악몽의 창고";
                case DispatchLocation.FateWarehouse: return "운명의 창고";
                case DispatchLocation.LaughterWarehouse: return "웃음의 창고";
                case DispatchLocation.TruthWarehouse: return "진실의 창고";
                case DispatchLocation.UnknownWarehouse: return "미지의 창고";
                case DispatchLocation.MagicLibraryOrganization: return "마도 서고 정돈";
                case DispatchLocation.MagicBarrierInspection: return "마력 장벽 유지 검사";
                case DispatchLocation.SpellbookCoverRestoration: return "마도서 표지 복원";
                case DispatchLocation.SealStabilityCheck: return "봉인구 안정성 확인";
                case DispatchLocation.MagicResiduePurification: return "마력 잔재 정화";
                default: return "알 수 없는 장소";
            }
        }

        /// <summary>
        /// 보상 데이터 가져오기
        /// </summary>
        private DispatchRewardTableData GetRewardData(int locationID, int timeID)
        {
            var rewardTable = CSVLoader.Instance.GetTable<DispatchRewardTableData>();
            if (rewardTable == null) return null;

            return rewardTable.FindAll(x =>
                x.Dispatch_Location_ID == locationID &&
                x.Dispatch_Time_ID == timeID
            ).FirstOrDefault();
        }

        /// <summary>
        /// 카테고리 데이터 가져오기 (Dispatch_ID로 조회)
        /// </summary>
        private DispatchCategoryData GetCategoryData(int dispatchID)
        {
            var categoryTable = CSVLoader.Instance.GetTable<DispatchCategoryData>();
            if (categoryTable == null)
            {
                Debug.LogError("[DispatchTestPanel] DispatchCategoryTable을 로드할 수 없습니다!");
                return null;
            }

            Debug.Log($"[DispatchTestPanel] DispatchCategoryTable 행 개수: {categoryTable.Count}");
            var result = categoryTable.FindAll(x => x.Dispatch_ID == dispatchID).FirstOrDefault();

            if (result == null)
            {
                Debug.LogError($"[DispatchTestPanel] Dispatch_ID {dispatchID}에 해당하는 카테고리를 찾을 수 없습니다!");
            }
            else
            {
                Debug.Log($"[DispatchTestPanel] 찾은 카테고리: Dispatch_ID={result.Dispatch_ID}, Category={result.Dispatch_Category}");
            }

            return result;
        }

        /// <summary>
        /// 파견하기 버튼 클릭
        /// </summary>
        private void OnDispatchButtonClicked()
        {
            if (dispatchManager == null)
            {
                Debug.LogError("[DispatchTestPanel] DispatchManager가 할당되지 않았습니다!");
                return;
            }

            var locationData = GetLocationData(testLocation);
            if (locationData == null)
            {
                Debug.LogError("[DispatchTestPanel] 장소 데이터를 찾을 수 없습니다!");
                return;
            }

            // Dispatch_ID로 카테고리 조회
            var categoryData = GetCategoryData(locationData.Dispatch_ID);
            if (categoryData == null)
            {
                Debug.LogError($"[DispatchTestPanel] Dispatch_ID {locationData.Dispatch_ID}에 대한 카테고리 데이터를 찾을 수 없습니다!");
                return;
            }

            // 파견 시작
            dispatchManager.StartDispatch(
                locationData.Dispatch_Location_ID,
                GetLocationName(testLocation),
                (DispatchType)categoryData.Dispatch_Category,
                currentSelectedHours
            );

            // 버튼 비활성화 (진행 중에는 중복 파견 불가)
            dispatchButton.interactable = false;

            // 일정 시간 후 버튼 다시 활성화 (테스트용)
            Invoke(nameof(EnableDispatchButton), 2f);
        }

        /// <summary>
        /// 파견하기 버튼 다시 활성화
        /// </summary>
        private void EnableDispatchButton()
        {
            dispatchButton.interactable = true;
        }

        private void OnDestroy()
        {
            // 이벤트 리스너 제거
            if (timeSlider != null)
                timeSlider.onValueChanged.RemoveListener(OnTimeSliderChanged);
            if (dispatchButton != null)
                dispatchButton.onClick.RemoveListener(OnDispatchButtonClicked);
        }
    }
}
