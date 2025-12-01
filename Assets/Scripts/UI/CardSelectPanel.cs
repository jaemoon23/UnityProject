using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;

namespace NovelianMagicLibraryDefense.UI
{
    /// <summary>
    /// LMJ: Universal card selection panel for both character and ability cards
    /// - Game Start: 2 character cards only
    /// - Level Up: 2 random cards (mix of character + ability cards)
    /// Selecting a card instantly processes it and closes panel
    /// </summary>
    public class CardSelectPanel : MonoBehaviour
    {
        [Header("Panel References")]
        [SerializeField] private GameObject panel;

        [Header("Card Container")]
        [SerializeField] private Transform cardContainer;

        [Header("Card Prefabs")]
        [SerializeField] private GameObject characterCardPrefab;
        [SerializeField] private GameObject statCard_AttackSpeed;
        [SerializeField] private GameObject statCard_Damage;
        [SerializeField] private GameObject statCard_ProjectileSpeed;
        [SerializeField] private GameObject statCard_Range;

        [Header("Settings")]
        [SerializeField] private bool pauseOnGameStart = true; // Pause for game start
        [SerializeField] private bool pauseOnLevelUp = true; // Pause for level-up

        [Header("Timer")]
        [SerializeField] private TextMeshProUGUI timerText; // CardSelectTimer 연결
        private const float SELECTION_TIME = 20f; // 20초 타이머
        private CancellationTokenSource selectionCts;
        private bool isCardSelected = false;

        // Events
        public event Action<CardData> OnCardSelected; // Fires when card is clicked

        private GameObject[] cardInstances;
        private CharacterPlacementManager placementManager;
        private float previousTimeScale = 1f;
        private bool isPaused = false;

        public enum CardType
        {
            Character,
            Ability
        }

        public struct CardData
        {
            public CardType Type;
            public int Id;

            public CardData(CardType type, int id)
            {
                Type = type;
                Id = id;
            }
        }

        private async void Awake()
        {
            // Find CharacterPlacementManager in scene
            placementManager = FindFirstObjectByType<CharacterPlacementManager>();
            if (placementManager == null)
            {
                Debug.LogWarning("[CardSelectPanel] CharacterPlacementManager not found in scene!");
            }
            else
            {
                // Wait for CharacterPlacementManager to finish preloading characters
                Debug.Log("[CardSelectPanel] Waiting for CharacterPlacementManager to preload characters...");
                int maxWaitFrames = 300; // 5 seconds at 60fps
                int frameCount = 0;

                while (!placementManager.IsPreloadComplete() && frameCount < maxWaitFrames)
                {
                    await Cysharp.Threading.Tasks.UniTask.Yield();
                    frameCount++;
                }

                if (placementManager.IsPreloadComplete())
                {
                    Debug.Log("[CardSelectPanel] CharacterPlacementManager preload complete!");
                }
                else
                {
                    Debug.LogWarning("[CardSelectPanel] CharacterPlacementManager preload timeout after 5 seconds!");
                }
            }

            // Don't initialize panel here - let it stay in its current state
            // Panel will be opened/closed by other scripts calling OpenForGameStart() or OpenForLevelUp()
            Debug.Log("[CardSelectPanel] Awake completed, ready to use");
        }

        /// <summary>
        /// Initialize panel to closed state
        /// </summary>
        private void InitializePanel()
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
            else
            {
                Debug.LogError("[CardSelectPanel] Panel GameObject not assigned!");
            }
        }

        /// <summary>
        /// Open panel for game start - 2 random character cards only
        /// </summary>
        public void OpenForGameStart()
        {
            if (panel == null) return;

            // Pause game for game start
            if (pauseOnGameStart)
            {
                previousTimeScale = Time.timeScale;
                Time.timeScale = 0f;
                isPaused = true;
            }

            // Get 2 random character IDs
            int[] characterIds = GetRandomCharacterIds(2);
            CardData[] cards = new CardData[2];
            cards[0] = new CardData(CardType.Character, characterIds[0]);
            cards[1] = new CardData(CardType.Character, characterIds[1]);

            panel.SetActive(true);
            CreateCards(cards);

            // 20초 타이머 시작
            StartSelectionTimer().Forget();

            Debug.Log("[CardSelectPanel] Opened for game start - 2 character cards (paused) + 20s timer");
        }

        /// <summary>
        /// Open panel for level up - 2 random cards (character or ability)
        /// If all slots are full, show only ability cards
        /// </summary>
        public void OpenForLevelUp()
        {
            if (panel == null) return;

            // Pause game if needed
            if (pauseOnLevelUp)
            {
                previousTimeScale = Time.timeScale;
                Time.timeScale = 0f;
                isPaused = true;
            }

            // Check if there are empty slots
            bool hasEmptySlot = placementManager != null && placementManager.HasEmptySlot();

            CardData[] cards;
            if (hasEmptySlot)
            {
                // Get 2 random cards (mix of character and ability)
                cards = GetRandomMixedCards(2);
                Debug.Log("[CardSelectPanel] Opened for level up - 2 random cards (slots available) + 20s timer");
            }
            else
            {
                // All slots full: show only ability cards
                cards = GetRandomAbilityCards(2);
                Debug.Log("[CardSelectPanel] Opened for level up - 2 ability cards (all slots full) + 20s timer");
            }

            panel.SetActive(true);
            CreateCards(cards);

            // 20초 타이머 시작
            StartSelectionTimer().Forget();
        }

        /// <summary>
        /// Open with specific cards (for testing or custom scenarios)
        /// </summary>
        public void OpenWithCards(CardData[] cards, bool pauseGame = false)
        {
            if (panel == null) return;
            if (cards == null || cards.Length != 2)
            {
                Debug.LogError("[CardSelectPanel] Must provide exactly 2 cards!");
                return;
            }

            // Pause game if requested
            if (pauseGame)
            {
                previousTimeScale = Time.timeScale;
                Time.timeScale = 0f;
                isPaused = true;
            }

            panel.SetActive(true);
            CreateCards(cards);
        }

        /// <summary>
        /// Close card selection panel
        /// </summary>
        public void Close()
        {
            if (panel == null) return;

            // 타이머 정지 및 정리
            StopTimer();

            panel.SetActive(false);

            // Resume game if it was paused
            if (isPaused)
            {
                Time.timeScale = previousTimeScale;
                isPaused = false;
            }

            ClearCards();

            // 상태 리셋
            isCardSelected = false;
        }

        /// <summary>
        /// Create 2 cards in the container
        /// </summary>
        private void CreateCards(CardData[] cards)
        {
            ClearCards();

            if (cardContainer == null)
            {
                Debug.LogError("[CardSelectPanel] Card container not assigned!");
                return;
            }

            cardInstances = new GameObject[2];

            for (int i = 0; i < 2; i++)
            {
                // 카드 타입/ID에 맞는 프리팹 선택
                GameObject prefab = GetCardPrefab(cards[i]);
                if (prefab == null)
                {
                    Debug.LogError($"[CardSelectPanel] Prefab not found for {cards[i].Type} (ID: {cards[i].Id})");
                    continue;
                }

                GameObject cardObj = Instantiate(prefab, cardContainer);
                cardObj.name = $"{cards[i].Type}Card_{cards[i].Id}";

                // 카드 텍스트 설정
                var cardText = cardObj.GetComponentInChildren<TextMeshProUGUI>();
                if (cardText != null)
                {
                    cardText.text = GetCardName(cards[i]);
                }

                // 버튼 이벤트 설정
                var cardButton = cardObj.GetComponent<UnityEngine.UI.Button>();
                if (cardButton != null)
                {
                    CardData cardData = cards[i];
                    cardButton.onClick.AddListener(() => OnCardClicked(cardData));
                }
                else
                {
                    Debug.LogWarning($"[CardSelectPanel] Card prefab missing Button component!");
                }

                cardInstances[i] = cardObj;
                Debug.Log($"[CardSelectPanel] Created {cards[i].Type} card: {GetCardName(cards[i])} (ID: {cards[i].Id})");
            }
        }

        /// <summary>
        /// 카드 타입/ID에 맞는 프리팹 반환
        /// </summary>
        private GameObject GetCardPrefab(CardData cardData)
        {
            if (cardData.Type == CardType.Character)
            {
                return characterCardPrefab;
            }

            // Ability(Stat) 카드 - ID별 프리팹 매핑
            return cardData.Id switch
            {
                1 => statCard_AttackSpeed,
                2 => statCard_Damage,
                3 => statCard_ProjectileSpeed,
                4 => statCard_Range,
                _ => statCard_AttackSpeed // fallback
            };
        }

        /// <summary>
        /// 카드 이름 반환 (캐릭터: CSV에서, 스탯: 하드코딩)
        /// </summary>
        private string GetCardName(CardData cardData)
        {
            if (cardData.Type == CardType.Character)
            {
                return GetCharacterNameFromCSV(cardData.Id);
            }

            // Stat 카드 이름
            return cardData.Id switch
            {
                1 => "공격 속도",
                2 => "공격력",
                3 => "투사체 속도",
                4 => "사거리",
                _ => $"Stat_{cardData.Id}"
            };
        }

        /// <summary>
        /// Handle card click - process card and close panel
        /// </summary>
        private void OnCardClicked(CardData cardData)
        {
            // 중복 선택 방지
            if (isCardSelected) return;

            // 타이머 정지
            StopTimer();

            // JML: 캐릭터 타입일 때만 CSV에서 이름 가져오기 (Issue #320)
            string cardName = cardData.Type == CardType.Character
                ? GetCharacterNameFromCSV(cardData.Id)
                : $"Ability_{cardData.Id}";
            Debug.Log($"[CardSelectPanel] 카드 선택: {cardName} (Type: {cardData.Type}, ID: {cardData.Id})");

            // Process card based on type
            if (cardData.Type == CardType.Character)
            {
                // Spawn character via CharacterPlacementManager
                if (placementManager != null)
                {
                    placementManager.SpawnCharacterById(cardData.Id);
                    Debug.Log($"[CardSelectPanel] 캐릭터 배치 완료: {cardName} (ID: {cardData.Id})");
                }
                else
                {
                    Debug.LogError("[CardSelectPanel] Cannot spawn character - CharacterPlacementManager is null!");
                }
            }
            else if (cardData.Type == CardType.Ability)
            {
                // TODO: Apply ability upgrade
                Debug.Log($"[CardSelectPanel] TODO: Apply ability upgrade ID {cardData.Id}");
                // Example: AbilityManager.ApplyAbility(cardData.Id);
            }

            // Fire event
            OnCardSelected?.Invoke(cardData);

            // Close panel
            Close();
        }

        /// <summary>
        /// JML: CSV에서 캐릭터 이름 가져오기 (Issue #320)
        /// </summary>
        private string GetCharacterNameFromCSV(int characterId)
        {
            if (CSVLoader.Instance == null || !CSVLoader.Instance.IsInit)
            {
                return $"Character_{characterId}";
            }

            var characterData = CSVLoader.Instance.GetData<CharacterData>(characterId);
            if (characterData == null)
            {
                return $"Character_{characterId}";
            }

            var stringData = CSVLoader.Instance.GetData<StringTable>(characterData.Character_Name_ID);
            return stringData?.Text ?? $"Character_{characterId}";
        }

        /// <summary>
        /// Clear all spawned cards
        /// </summary>
        private void ClearCards()
        {
            if (cardContainer == null) return;

            foreach (Transform child in cardContainer)
            {
                Destroy(child.gameObject);
            }

            cardInstances = null;
        }

        #region Timer Methods

        /// <summary>
        /// 20초 타이머 시작 (Time.timeScale=0에서도 동작)
        /// </summary>
        private async UniTask StartSelectionTimer()
        {
            isCardSelected = false;
            selectionCts?.Dispose();
            selectionCts = new CancellationTokenSource();
            float remainingTime = SELECTION_TIME;

            // 초기 타이머 표시
            UpdateTimerText(remainingTime);

            try
            {
                while (remainingTime > 0 && !isCardSelected)
                {
                    // 1초 대기 (ignoreTimeScale=true로 Time.timeScale=0에서도 동작)
                    await UniTask.Delay(1000, ignoreTimeScale: true, cancellationToken: selectionCts.Token);
                    remainingTime -= 1f;
                    UpdateTimerText(remainingTime);
                }

                // 타임아웃 시 랜덤 선택
                if (!isCardSelected)
                {
                    Debug.Log("[CardSelectPanel] 20초 타임아웃! 랜덤 카드 선택");
                    AutoSelectCard();
                }
            }
            catch (OperationCanceledException)
            {
                // 타이머 취소됨 (카드 선택 시 정상 동작)
                Debug.Log("[CardSelectPanel] 타이머 취소됨 (카드 선택됨)");
            }
            finally
            {
                selectionCts?.Dispose();
                selectionCts = null;
            }
        }

        /// <summary>
        /// 타이머 텍스트 업데이트
        /// </summary>
        private void UpdateTimerText(float remainingTime)
        {
            if (timerText != null)
            {
                timerText.text = $"{Mathf.CeilToInt(remainingTime)}s";
            }
        }

        /// <summary>
        /// 타이머 정지
        /// </summary>
        private void StopTimer()
        {
            isCardSelected = true;
            selectionCts?.Cancel();
            UpdateTimerText(0);
        }

        /// <summary>
        /// 타임아웃 시 랜덤으로 카드 선택
        /// </summary>
        private void AutoSelectCard()
        {
            if (cardInstances == null || cardInstances.Length == 0)
            {
                Debug.LogWarning("[CardSelectPanel] 선택할 카드가 없습니다!");
                Close();
                return;
            }

            // 랜덤으로 카드 인덱스 선택
            int randomIndex = UnityEngine.Random.Range(0, cardInstances.Length);

            // 해당 카드의 버튼 클릭 시뮬레이션
            var cardButton = cardInstances[randomIndex]?.GetComponent<UnityEngine.UI.Button>();
            if (cardButton != null)
            {
                Debug.Log($"[CardSelectPanel] 랜덤 선택: 카드 {randomIndex + 1}");
                cardButton.onClick.Invoke();
            }
            else
            {
                Debug.LogWarning("[CardSelectPanel] 랜덤 선택된 카드에 버튼이 없습니다!");
                Close();
            }
        }

        #endregion

        /// <summary>
        /// JML: DeckManager에서 캐릭터 ID 가져오기 (Issue #320)
        /// </summary>
        private int[] GetRandomCharacterIds(int count)
        {
            // DeckManager에서 덱 가져오기
            var deck = DeckManager.Instance?.GetValidCharacters();

            // JML: 덱 상태 디버그 로그
            Debug.Log($"[CardSelectPanel] 덱 상태: {(deck != null ? $"[{string.Join(", ", deck)}] (총 {deck.Count}개)" : "null")}");

            if (deck == null || deck.Count == 0)
            {
                Debug.LogWarning("[CardSelectPanel] DeckManager가 없거나 덱이 비어있습니다!");
                return new int[0];
            }

            // 덱에서 랜덤하게 count개 선택
            int[] selected = new int[Math.Min(count, deck.Count)];
            var shuffled = new System.Collections.Generic.List<int>(deck);

            // Fisher-Yates shuffle
            for (int i = shuffled.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                int temp = shuffled[i];
                shuffled[i] = shuffled[j];
                shuffled[j] = temp;
            }

            for (int i = 0; i < selected.Length; i++)
            {
                selected[i] = shuffled[i];
            }

            Debug.Log($"[CardSelectPanel] DeckManager에서 캐릭터 {selected.Length}개 선택: [{string.Join(", ", selected)}]");
            return selected;
        }

        /// <summary>
        /// JML: DeckManager 연동된 혼합 카드 (Issue #320)
        /// </summary>
        private CardData[] GetRandomMixedCards(int count)
        {
            // DeckManager에서 캐릭터 풀 가져오기
            var characterPool = DeckManager.Instance?.GetValidCharacters();
            int[] abilityPool = { 1, 2, 3, 4, 5 };  // TODO: AbilityTable 연동

            CardData[] cards = new CardData[count];

            for (int i = 0; i < count; i++)
            {
                // 50% 확률로 캐릭터 또는 어빌리티
                bool isCharacter = UnityEngine.Random.value > 0.5f;

                if (isCharacter && characterPool != null && characterPool.Count > 0)
                {
                    int randomId = characterPool[UnityEngine.Random.Range(0, characterPool.Count)];
                    cards[i] = new CardData(CardType.Character, randomId);
                }
                else
                {
                    int randomId = abilityPool[UnityEngine.Random.Range(0, abilityPool.Length)];
                    cards[i] = new CardData(CardType.Ability, randomId);
                }
            }

            return cards;
        }

        /// <summary>
        /// Get N random ability cards only (when all character slots are full)
        /// </summary>
        private CardData[] GetRandomAbilityCards(int count)
        {
            // TODO: Get from CardTableData or config
            // For now, hardcoded pool
            int[] abilityPool = { 1, 2, 3, 4, 5 };

            CardData[] cards = new CardData[count];
            int[] shuffled = (int[])abilityPool.Clone();

            // Fisher-Yates shuffle
            for (int i = shuffled.Length - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                int temp = shuffled[i];
                shuffled[i] = shuffled[j];
                shuffled[j] = temp;
            }

            for (int i = 0; i < count; i++)
            {
                cards[i] = new CardData(CardType.Ability, shuffled[i % shuffled.Length]);
            }

            return cards;
        }

        /// <summary>
        /// Check if panel is currently open
        /// </summary>
        public bool IsOpen => panel != null && panel.activeSelf;
    }
}
