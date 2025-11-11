# 🧹 로컬 브랜치 정리 스크립트

원격에서 삭제된 브랜치를 로컬에서도 정리하는 스크립트입니다.

## 🚀 사용 방법

### 기본 사용
```bash
bash scripts/cleanup-branches.sh
```

### 실행 결과 예시
```bash
$ bash scripts/cleanup-branches.sh
🧹 Cleaning up stale local branches...

📡 Fetching remote changes...

🔍 Finding stale branches...

Found the following stale branches:
  ❌ feature/old-work
  ❌ bugfix/fixed-bug

❓ Delete these branches? (y/N): y
  ✅ Deleted: feature/old-work
  ✅ Deleted: bugfix/fixed-bug

🎉 Cleanup completed!
```

---

## 📋 무엇을 정리하나요?

### 삭제 대상
- ✅ 원격(origin)에서 이미 삭제된 브랜치
- ✅ merge되고 삭제된 브랜치

### 삭제 안 되는 것
- ❌ 현재 체크아웃된 브랜치
- ❌ 원격에 아직 존재하는 브랜치

---

## 💡 언제 실행하나요?

### 추천 타이밍
- 📅 **주 1회**: 정기적으로 정리
- 🎯 **PR merge 후**: 여러 브랜치 작업 후
- 🧐 **브랜치 목록이 지저분할 때**: `git branch` 결과가 많을 때

### 실행 전 확인
```bash
# 현재 로컬 브랜치 목록 확인
git branch

# 원격에 없는 브랜치 미리 확인
git branch -vv | grep ': gone]'
```

---

## 🛡️ 안전 기능

### 1. 확인 단계
- 삭제 전 목록 표시
- y/N 확인 필요 (기본값: N)
- 실수로 삭제 방지

### 2. 백업 유지
- 자동 실행 안 됨
- 원할 때만 수동 실행
- 삭제 전 검토 가능

---

## ⚠️ 주의사항

### 1. 커밋 안 한 작업 확인
```bash
# 작업 중인 브랜치가 있는지 확인
git status

# 커밋 안 한 변경사항이 있으면 먼저 커밋
git add .
git commit -m "WIP: Save work"
```

### 2. Push 안 한 커밋 확인
```bash
# Push 안 한 커밋이 있는지 확인
git log origin/your-branch..your-branch

# Push 안 한 커밋이 있으면 먼저 push
git push origin your-branch
```

---

## 🔧 문제 해결

### "Permission denied" 오류
```bash
# 실행 권한 추가
chmod +x scripts/cleanup-branches.sh

# 다시 실행
bash scripts/cleanup-branches.sh
```

### 특정 브랜치만 남기고 싶을 때
```bash
# 스크립트 실행 후 N 선택
# 수동으로 개별 삭제
git branch -D branch-name
```

### 삭제 취소하고 싶을 때
```bash
# 최근 삭제된 브랜치 복구 (30일 이내)
git reflog
git checkout -b branch-name commit-hash
```

---

## 💻 팀원 사용 가이드

### 초기 설정
```bash
# 1. 최신 코드 받기
git pull origin main

# 2. 스크립트 확인
ls scripts/cleanup-branches.sh

# 끝! 별도 설정 필요 없음
```

### 정기 사용
```bash
# 주 1회 실행 권장
bash scripts/cleanup-branches.sh
```

---

## 🎯 고급 사용법

### 자동 Yes (확인 없이 바로 삭제)
```bash
yes | bash scripts/cleanup-branches.sh
```
⚠️ **위험**: 확인 없이 바로 삭제되므로 주의!

### 삭제 전 목록만 확인
```bash
git fetch --prune
git branch -vv | grep ': gone]'
```

---

## 📝 vs Git Hooks 비교

| 특징 | 이 스크립트 | Git Hooks |
|------|------------|----------|
| 설치 필요 | ❌ 없음 | ✅ 필요 |
| 실행 방식 | 수동 | 자동 |
| 안전성 | ✅ 확인 후 삭제 | ⚠️ 자동 삭제 |
| 백업 | ✅ 유지 가능 | ❌ 자동 삭제 |
| 공유 | ✅ Git 추적 | ❌ 개별 설정 |

---

## 📞 지원

문제가 있으면:
1. 이 README 다시 읽기
2. 팀 리드(이명진)에게 문의
3. GitHub Issues에 등록

---

## 📅 변경 이력

- 2024-11-11: 초기 버전 작성 (Git Hooks에서 스크립트로 변경)
