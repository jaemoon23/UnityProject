# 🔄 Branch Rebase Workflow

## 빠른 설정

### 1단계: Slack Webhook Secret 추가

1. Repository → **Settings** → **Secrets and variables** → **Actions**
2. **New repository secret** 클릭
3. 다음 정보 입력:
   ```
   Name: SLACK_WEBHOOK_URL
   Value: https://hooks.slack.com/services/YOUR/WEBHOOK/URL
   ```

### 2단계: GitHub Token 권한 설정

1. Repository → **Settings** → **Actions** → **General**
2. **Workflow permissions** 섹션에서:
   - ✅ **"Read and write permissions"** 선택
   - ✅ **"Allow GitHub Actions to create and approve pull requests"** 체크

---

## 🚀 사용 방법

### 워크플로우 실행

1. **https://github.com/leemjmorris/Novelian-Magic-Library-Deffense/actions**
2. 좌측 메뉴에서 **"Rebase All Branches to Main"** 선택
3. **"Run workflow"** 버튼 클릭
4. 옵션 설정:
   - **Branch pattern**: 
     - `feature/*` - feature 브랜치만
     - `bugfix/*` - bugfix 브랜치만
     - `hotfix/*` - hotfix 브랜치만
     - `all` - main 제외 모든 브랜치
   - **Base branch**: 기준이 될 브랜치 (기본: main)
   - **Force push**: rebase 후 강제 푸시 여부 (기본: true)

---

## 📋 동작 방식

### 처리 과정

모든 브랜치를 **origin/main** 기준으로 rebase:

1. ✅ 브랜치 체크아웃
2. ✅ **조건 1**: Uncommitted changes 확인 → 있으면 **스킵**
3. ✅ **조건 2**: origin/main으로 rebase 시도
4. ✅ **조건 3**: 충돌 발생 시 → **자동 abort 후 스킵**
5. ✅ 성공 시 → Force push

### 결과 분류

- **✅ Success**: Rebase 및 push 성공
- **❌ Failed**: Rebase 충돌 또는 push 실패
- **⚠️ Skipped**: Uncommitted changes 존재

---

## 🔔 Slack 알림

### 알림 내용
- 📊 상태 요약 (성공/실패/스킵 개수)
- 📝 각 브랜치별 상세 결과
- 🔗 워크플로우 링크 버튼

### 알림 색상
- 🟢 **초록색**: 모든 브랜치 성공
- 🔴 **빨간색**: 모든 브랜치 실패  
- 🟡 **노란색**: 부분 성공 또는 모두 스킵

---

## 💡 사용 팁

### 안전한 테스트 방법

**1단계: 테스트 모드**
```
Branch pattern: feature/*
Force push: false (체크 해제)
```
→ 어떤 브랜치에서 충돌이 발생하는지 파악

**2단계: 수동 처리**
```bash
git checkout feature/conflict-branch
git rebase origin/main
# 충돌 해결
git push origin feature/conflict-branch --force-with-lease
```

**3단계: 실제 적용**
```
Branch pattern: feature/*
Force push: true (체크)
```
→ 충돌 없는 브랜치들 일괄 rebase

---

## 🛡️ 안전 장치

1. **`--force-with-lease`**: 원격에 새 커밋 있으면 push 실패
2. **자동 abort**: 충돌 발생 시 rebase 전 상태로 복구
3. **Uncommitted changes 체크**: 작업 중인 브랜치 보호

---

## 📝 주의사항

1. **Force Push 위험**: 다른 팀원이 작업 중인 브랜치는 사전 확인 필요
2. **대량 브랜치**: `all` 옵션은 신중하게 사용
3. **충돌 해결**: 자동으로 해결 불가, 수동 처리 필요

---

## 🐛 문제 해결

### "Permission denied" 오류
→ Settings → Actions → General → Workflow permissions → "Read and write permissions"

### Slack 알림 안 옴  
→ `SLACK_WEBHOOK_URL` Secret 확인

---

## 📞 지원

문제가 발생하면 GitHub Issues에 남겨주세요!
