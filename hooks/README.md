# 🧹 로컬 브랜치 자동 삭제 Git Hook

원격에서 삭제된 브랜치를 로컬에서도 자동으로 삭제합니다.

## 🚀 빠른 설치 (팀원 필수)

### 1단계: 프로젝트 최신 버전 받기
```bash
git pull origin main
```

### 2단계: Hook 설치 스크립트 실행
```bash
cd Novelian-Magic-Library-Deffense  # 프로젝트 루트로 이동
bash hooks/install.sh
```

**완료!** 이제 `git pull` 할 때마다 자동으로 정리됩니다.

---

## 📋 동작 방식

### 언제 실행되나요?
- `git pull` 할 때마다 자동 실행
- `git merge` 할 때마다 자동 실행

### 무엇을 삭제하나요?
- ✅ 원격에서 삭제된 브랜치
- ❌ main/master 브랜치 (안전)
- ❌ 현재 작업 중인 브랜치 (안전)

### 예시
```bash
$ git pull
🧹 Checking for stale local branches...
  ❌ Deleting: feature/old-feature (not on remote)
  ❌ Deleting: bugfix/fixed-bug (not on remote)
  ✅ Deleted 2 stale branch(es)
```

---

## 🔧 수동 설치 (자동 스크립트 안 쓸 경우)

### Windows
```cmd
copy hooks\post-merge .git\hooks\post-merge
```

### Mac/Linux
```bash
cp hooks/post-merge .git/hooks/post-merge
chmod +x .git/hooks/post-merge
```

---

## ⚠️ 주의사항

### 1. 각 팀원이 개별 설치 필요
- Git Hooks는 원격에 push 안 됨
- **새 팀원이 들어오면** → 이 문서 공유 + 설치 필수

### 2. 저장 안 된 작업 주의
- 원격에서 삭제된 브랜치에 커밋 안 한 작업이 있으면 **날아갑니다**
- 항상 커밋 후 push 하는 습관 중요

### 3. PC 포맷 시
- `.git` 폴더가 날아가므로 다시 설치 필요

---

## 🧪 테스트 방법

### 테스트 시나리오
1. 테스트 브랜치 생성 및 push
```bash
git checkout -b test/auto-delete
git push origin test/auto-delete
```

2. GitHub에서 브랜치 삭제
```
GitHub → Branches → test/auto-delete 삭제
```

3. 로컬에서 pull
```bash
git checkout main
git pull
```

4. 결과 확인
```bash
# test/auto-delete 브랜치가 자동 삭제되었는지 확인
git branch
```

---

## 🛠️ 문제 해결

### Hook이 실행 안 됨
```bash
# 실행 권한 확인
ls -la .git/hooks/post-merge

# 권한 없으면 추가
chmod +x .git/hooks/post-merge
```

### Hook 비활성화하고 싶을 때
```bash
# 임시 비활성화
mv .git/hooks/post-merge .git/hooks/post-merge.disabled

# 다시 활성화
mv .git/hooks/post-merge.disabled .git/hooks/post-merge
```

### Hook 완전 삭제
```bash
rm .git/hooks/post-merge
```

---

## 📂 파일 구조

```
Novelian-Magic-Library-Deffense/
├── hooks/
│   ├── post-merge       # 실제 hook 스크립트
│   ├── install.sh       # 설치 스크립트
│   └── README.md        # 이 문서
└── .git/
    └── hooks/
        └── post-merge   # 설치 후 여기에 복사됨 (Git이 실행)
```

---

## 💡 팁

### 현재 설치 상태 확인
```bash
# Hook이 설치되어 있는지 확인
ls -la .git/hooks/post-merge
```

### 수동으로 실행해보기
```bash
# pull 안 해도 테스트 가능
.git/hooks/post-merge
```

---

## 📞 문제 발생 시

1. 이 README 다시 읽기
2. 팀 리드(이명진)에게 문의
3. GitHub Issues에 등록

---

## 📝 변경 이력

- 2024-11-11: 초기 버전 작성
