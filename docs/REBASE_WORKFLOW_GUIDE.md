# 🔄 Fork Branch Rebase Workflow

## 빠른 설정 가이드

### 1단계: Slack Webhook Secret 추가

1. Repository → **Settings** → **Secrets and variables** → **Actions**
2. **New repository secret** 클릭
3. 다음 정보 입력:
   ```
   Name: SLACK_WEBHOOK_URL
   Value: https://hooks.slack.com/services/YOUR/WEBHOOK/URL
   ```

### 2단계: Upstream 저장소 설정

`.github/workflows/rebase-fork-branches.yml` 파일의 28번째 줄 수정:

```yaml
env:
  UPSTREAM_REPO: '원본저장소owner/원본저장소name'
```

**⚠️ 중요**: 이 레파지토리가 fork가 아니라면, upstream 저장소 URL을 정확히 입력해야 합니다!

### 3단계: GitHub Token 권한 설정

1. Repository → **Settings** → **Actions** → **General**
2. **Workflow permissions** 섹션에서:
   - ✅ **"Read and write permissions"** 선택
   - ✅ **"Allow GitHub Actions to create and approve pull requests"** 체크

---

## 🚀 사용 방법

1. GitHub 레파지토리 → **Actions** 탭
2. 좌측에서 **"Rebase Fork Branches"** 선택
3. **"Run workflow"** 버튼 클릭
4. 옵션 설정 후 실행

---

## 📊 결과 확인

- GitHub Actions Summary 페이지
- Slack 알림 메시지

자세한 내용은 워크플로우 실행 후 확인하세요!
