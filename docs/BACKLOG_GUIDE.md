# 📋 백로그 관리 가이드

## 개요

이 가이드는 **Novelian Magic Library Defense** 프로젝트의 백로그 관리 방법을 설명합니다.

---

## 🎯 백로그란?

**백로그(Backlog)**는 아직 특정 마일스톤이나 스프린트에 할당되지 않은 작업들의 저장소입니다.

### 백로그의 목적
- 모든 아이디어와 작업을 한 곳에 수집
- 우선순위에 따라 작업 선택
- 스프린트/마일스톤 계획 시 참고

---

## 🏷️ 레이블 시스템

### 백로그 관련 레이블

| 레이블 | 설명 | 사용 시점 |
|--------|------|-----------|
| `backlog` | 백로그에 있는 작업 | 마일스톤 미할당 작업 |
| `ready` | 작업 준비 완료 | 요구사항 명확화 완료 |
| `blocked` | 진행 차단된 작업 | 의존성 등으로 대기 중 |
| `wontfix` | 진행하지 않을 작업 | 우선순위 낮음 또는 불필요 |

### 우선순위 레이블

| 레이블 | 색상 | 설명 |
|--------|------|------|
| `priority: critical` | 🔴 빨강 | 즉시 처리 필요 |
| `priority: high` | 🟠 주황 | 다음 스프린트 우선 |
| `priority: medium` | 🟡 노랑 | 일반 우선순위 |
| `priority: low` | 🟢 초록 | 여유 있을 때 처리 |

---

## 📊 워크플로우

### 1. 백로그에 작업 추가

```bash
# 이슈 생성 시
- 마일스톤: 할당하지 않음
- 레이블: backlog + priority 레이블 추가
- 담당자: 미정 또는 팀 리더
```

**예시:**
```markdown
제목: 스킬 쿨다운 UI 추가
레이블: backlog, enhancement, priority: medium
마일스톤: (없음)
```

### 2. 스프린트 계획 회의

**매주 또는 마일스톤 시작 시:**

1. 백로그 검토
   - `backlog` 레이블 이슈 필터링
   - 우선순위 순으로 정렬

2. 작업 선택
   - 팀 역량 고려
   - 의존성 확인
   - 마일스톤 목표 부합 여부

3. 마일스톤 할당
   ```bash
   # 선택된 이슈에 마일스톤 할당
   - backlog 레이블 제거
   - ready 레이블 추가
   - 담당자 지정
   ```

### 3. 작업 진행

```
Backlog → Ready → In Progress → In Review → Done
```

---

## 🎨 GitHub Projects 보드

### 컬럼 구성

| 컬럼 | 설명 | 이슈 상태 |
|------|------|-----------|
| **Backlog** | 백로그 작업 | `backlog` 레이블 |
| **Ready** | 준비 완료 | `ready` 레이블 + 마일스톤 할당 |
| **In Progress** | 진행 중 | 담당자 할당 + 작업 중 |
| **In Review** | 리뷰 중 | PR 생성됨 |
| **Done** | 완료 | 이슈 닫힘 |

### 보드 사용법

1. **드래그 앤 드롭**
   - Backlog → Ready: 스프린트에 추가
   - Ready → In Progress: 작업 시작
   - In Progress → In Review: PR 생성

2. **자동화 (GitHub Actions)**
   - 이슈 생성 시 자동으로 Backlog 추가
   - PR 머지 시 자동으로 Done 이동

---

## 🔍 필터링 & 검색

### 백로그 이슈만 보기

```
is:issue is:open no:milestone label:backlog
```

### 우선순위별 필터링

```
is:issue is:open label:backlog label:"priority: high"
```

### 특정 타입 + 백로그

```
is:issue is:open label:backlog label:bug
```

---

## 📅 정기 리뷰

### 주간 백로그 그루밍 (매주 금요일)

- [ ] 새로운 이슈 검토
- [ ] 우선순위 재조정
- [ ] 중복 이슈 통합
- [ ] 오래된 이슈 정리 (3개월 이상)

### 마일스톤 계획 회의 (마일스톤 시작 전)

- [ ] 백로그에서 작업 선택
- [ ] 팀원별 작업 할당
- [ ] 예상 작업 시간 산정
- [ ] 의존성 파악

---

## 💡 Best Practices

### ✅ 좋은 예

```markdown
제목: [Feature] 캐릭터 스킬 쿨다운 UI 구현
내용:
- 스킬 아이콘 위에 쿨다운 타이머 표시
- 쿨다운 중 아이콘 반투명 처리
- 쿨다운 완료 시 이펙트

레이블: backlog, enhancement, priority: high, UI
```

### ❌ 피해야 할 것

```markdown
제목: UI 수정
내용: UI 좀 고쳐주세요

레이블: backlog
```

---

## 🔗 관련 문서

- [코딩 컨벤션](../README.md#coding-conventions)
- [이슈 템플릿](.github/ISSUE_TEMPLATE/)
- [PR 템플릿](.github/pull_request_template.md)

---

**Last Updated**: 2025-11-09
