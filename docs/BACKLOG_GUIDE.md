# 백로그 관리 시스템 가이드

## 📋 백로그 시스템 개요

이 프로젝트는 **레이블 기반 백로그**와 **GitHub Projects 칸반 보드**를 함께 사용합니다.

## 🏷️ 레이블 시스템

### 작업 상태 레이블
- **📥 backlog** - 백로그에 있는 작업 (마일스톤 미할당)
- **✅ ready** - 작업 준비 완료 (마일스톤 할당됨)
- **🚀 in-progress** - 현재 진행 중
- **👀 in-review** - 리뷰 중
- **✨ blocked** - 차단됨 (의존성 문제 등)

### 우선순위 레이블
- **🔴 priority: critical** - 최우선
- **🟡 priority: high** - 높음
- **🔵 priority: medium** - 중간
- **⚪ priority: low** - 낮음

## 🔄 자동화 흐름

### 1. 새 이슈 생성시
- 마일스톤이 없으면 자동으로 `📥 backlog` 레이블 추가

### 2. 마일스톤 할당시
- `📥 backlog` 레이블 제거
- `✅ ready` 레이블 추가

### 3. 마일스톤 제거시
- `✅ ready` 레이블 제거
- `📥 backlog` 레이블 추가

## 📊 GitHub Projects 보드 설정

### 1단계: 프로젝트 생성
```
1. GitHub 레포지토리 → Projects 탭
2. "New project" 클릭
3. "Board" 템플릿 선택
4. 이름: "Sprint Board" 또는 "Development Board"
```

### 2단계: 컬럼 구성
다음 컬럼들을 만드세요:

```
┌──────────┬─────────┬─────────────┬────────────┬──────┐
│ Backlog  │  Ready  │ In Progress │ In Review  │ Done │
├──────────┼─────────┼─────────────┼────────────┼──────┤
│ 마일스톤 │ 스프린트│   작업 중   │  리뷰 중   │ 완료 │
│ 미할당   │ 준비완료│             │            │      │
└──────────┴─────────┴─────────────┴────────────┴──────┘
```

### 3단계: 자동화 설정
각 컬럼에서 "..." → "Workflows" 설정:

**Backlog 컬럼:**
- When: Issues are added to project
- Set: Status to Backlog

**Ready 컬럼:**
- When: Issues are moved to Ready
- Set: Label to "✅ ready"

**In Progress 컬럼:**
- When: Issues are moved to In Progress
- Set: Label to "🚀 in-progress"

**In Review 컬럼:**
- When: Pull request is opened
- Move to: In Review
- Set: Label to "👀 in-review"

**Done 컬럼:**
- When: Issues are closed
- Move to: Done

### 4단계: 레이블 필터 추가
프로젝트에서 Filter 사용:
```
label:"📥 backlog"  → 백로그 보기
label:"✅ ready"    → 준비된 작업 보기
milestone:@current  → 현재 스프린트 보기
```

## 💡 사용 방법

### 백로그에서 스프린트로 이동하기
1. **방법 1 (자동):** 이슈에 마일스톤 할당
2. **방법 2 (수동):** Projects 보드에서 Backlog → Ready로 드래그

### 작업 우선순위 지정하기
```bash
# 우선순위 레이블 추가
🔴 priority: critical  # 최우선
🟡 priority: high      # 높음
🔵 priority: medium    # 중간
⚪ priority: low       # 낮음
```

### 백로그 보기
- GitHub Issues에서 `label:"📥 backlog"` 필터
- 또는 Projects 보드의 Backlog 컬럼 확인

## 📈 워크플로우 예시

### 새로운 기능 아이디어
```
1. Issue 생성 → 자동으로 "📥 backlog" 추가
2. 기획 회의에서 검토
3. 다음 스프린트에 포함 결정
4. Milestone 할당 → 자동으로 "✅ ready"로 변경
5. 개발자 할당 및 작업 시작
6. PR 생성 → 자동으로 "👀 in-review"
7. 리뷰 완료 후 merge → "Done"
```

### 백로그 정리
정기적으로 (주 1회 권장):
1. Backlog 이슈 검토
2. 우선순위 레이블 업데이트
3. 오래된/불필요한 이슈 close
4. 다음 스프린트 계획

## 🔗 유용한 링크

- [GitHub Projects 문서](https://docs.github.com/en/issues/planning-and-tracking-with-projects)
- [레이블 관리](https://docs.github.com/en/issues/using-labels-and-milestones-to-track-work)
