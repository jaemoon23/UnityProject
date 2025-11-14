# Unity MCP + Claude Code (VSCode) 연동 가이드

Unity 프로젝트에서 MCP(Model Context Protocol) 서버를 설치하고 VSCode의 Claude Code와 연동하는 방법을 단계별로 안내합니다.

---

## 목차
1. [Claude CLI 설치](#1-claude-cli-설치)
2. [uv 설치 및 환경 변수 설정](#2-uv-설치-및-환경-변수-설정)
3. [Unity MCP 서버 설치](#3-unity-mcp-서버-설치)
4. [MCP 설정 파일 구성](#4-mcp-설정-파일-구성)
5. [VSCode Claude Code 설정](#5-vscode-claude-code-설정)
6. [연결 확인](#6-연결-확인)
7. [멀티 컴퓨터 설정](#7-멀티-컴퓨터-설정)
8. [트러블슈팅](#8-트러블슈팅)

---

## 1. Claude CLI 설치

### 1-1. Node.js 설치 확인
먼저 Node.js가 설치되어 있는지 확인합니다.

```bash
node --version
npm --version
```

Node.js가 없다면 [Node.js 공식 홈페이지](https://nodejs.org/)에서 LTS 버전을 다운로드하여 설치하세요.

### 1-2. Claude CLI 설치

명령 프롬프트 또는 PowerShell을 **관리자 권한**으로 실행한 후:

```bash
npm install -g @anthropic-ai/claude-code
```

### 1-3. 설치 확인

```bash
claude --version
```

버전이 정상적으로 출력되면 설치 완료입니다.

---

## 2. uv 설치 및 환경 변수 설정

MCP 서버는 Python 환경이 필요하며, `uv`라는 패키지 관리자를 사용합니다.

### 2-1. uv 설치

#### Windows (PowerShell)
```powershell
powershell -ExecutionPolicy ByPass -c "irm https://astral.sh/uv/install.ps1 | iex"
```

### 2-2. PATH 환경 변수에 uv 추가

설치 후 `uv` 명령어를 어디서나 사용할 수 있도록 PATH에 추가해야 합니다.

#### 단계별 가이드 (Windows)

1. **시스템 속성 열기**
   - `Win + R` 키를 누르고 `sysdm.cpl` 입력 후 Enter

2. **환경 변수 설정 창 열기**
   - **"고급"** 탭 클릭
   - **"환경 변수(N)..."** 버튼 클릭

3. **PATH 변수 편집**
   - 하단 **"시스템 변수(S)"** 영역에서 `Path` 찾기
   - `Path` 선택 후 **"편집(I)..."** 버튼 클릭

4. **새 경로 추가**
   - **"새로 만들기(N)"** 버튼 클릭
   - 아래 경로 중 하나를 입력:

**데스크톱 환경 (사용자명: happy)**
```
C:\Users\happy\.local\bin
```

**노트북 환경 (사용자명: LMJ)**
```
C:\Users\LMJ\AppData\Local\Microsoft\WinGet\Links
```

또는 자신의 사용자명으로:
```
C:\Users\[사용자명]\.local\bin
```

5. **설정 저장**
   - **"확인"** 버튼을 3번 눌러 모든 창 닫기

### 2-3. PATH 설정 확인

**⚠️ 중요: 새 명령 프롬프트 창을 열어야 합니다** (기존 창은 환경 변수를 인식하지 못합니다)

```bash
uv --version
```

버전이 정상적으로 출력되면 성공입니다!

---

## 3. Unity MCP 서버 설치

### 3-1. 설치 경로 생성

```bash
mkdir %LOCALAPPDATA%\UnityMCP
cd %LOCALAPPDATA%\UnityMCP
```

### 3-2. Unity MCP 서버 클론

```bash
git clone https://github.com/coplaydev/UnityMcpServer.git
cd UnityMcpServer
```

### 3-3. 의존성 설치

```bash
uv sync
```

---

## 4. MCP 설정 파일 구성

### 4-1. 설정 파일 위치

VSCode Claude Code의 MCP 설정 파일은 다음 위치에 있습니다:

```
%APPDATA%\Code\User\globalStorage\claude-code\config.json
```

또는 직접 경로:
```
C:\Users\[사용자명]\AppData\Roaming\Code\User\globalStorage\claude-code\config.json
```

### 4-2. 설정 파일 내용

설정 파일을 열어서 다음 내용을 추가하거나 수정합니다:

```json
{
  "mcpServers": {
    "UnityMCP": {
      "command": "uv",
      "args": [
        "run",
        "--directory",
        "%LOCALAPPDATA%\\UnityMCP\\UnityMcpServer\\src",
        "server.py"
      ],
      "type": "stdio"
    }
  }
}
```

#### 주요 포인트
- ✅ `%LOCALAPPDATA%`는 Windows가 자동으로 각 사용자 경로로 변환합니다
- ✅ 멀티 컴퓨터 환경에서도 **동일한 설정** 사용 가능
- ✅ Git이나 클라우드로 설정 동기화 가능

---

## 5. VSCode Claude Code 설정

### 5-1. Claude Code 확장 설치

1. VSCode 열기
2. 확장(Extensions) 탭 열기 (`Ctrl + Shift + X`)
3. "Claude Code" 검색
4. **Anthropic의 공식 확장**인지 확인 후 설치

### 5-2. API Key 설정

1. VSCode에서 `Ctrl + Shift + P`
2. "Claude Code: Set API Key" 입력
3. Anthropic API Key 입력

API Key는 [Anthropic Console](https://console.anthropic.com/)에서 발급받을 수 있습니다.

### 5-3. VSCode 재시작

설정을 적용하기 위해 VSCode를 완전히 종료하고 다시 실행합니다.

---

## 6. 연결 확인

### 6-1. Unity 프로젝트 열기

Unity Editor를 실행하고 프로젝트를 엽니다.

### 6-2. VSCode에서 Claude Code 실행

1. VSCode에서 Unity 프로젝트 폴더 열기
2. `Ctrl + Shift + P` → "Claude Code: Start Chat" 선택
3. Claude Code 창이 열리면 다음 명령어 시도:

```
Unity Editor의 상태를 확인해줘
```

또는

```
현재 열려있는 씬의 정보를 알려줘
```

### 6-3. MCP 연결 확인

Claude Code가 Unity MCP 서버와 정상적으로 연결되면:
- Unity Editor의 정보를 실시간으로 가져올 수 있습니다
- 스크립트 생성, GameObject 조작 등이 가능합니다

---

## 7. 멀티 컴퓨터 설정

서로 다른 컴퓨터(데스크톱/노트북)에서 작업할 때의 설정 방법입니다.

### 7-1. 문제 상황
사용자 계정명이 다르면 경로가 달라져서 설정 충돌이 발생할 수 있습니다.

### 7-2. 해결 방법
환경 변수(`%LOCALAPPDATA%`)를 사용하면 자동으로 각 컴퓨터의 경로에 맞게 변환됩니다.

### 7-3. 각 컴퓨터별 PATH 설정

#### 데스크톱 (사용자명: happy)
```
C:\Users\happy\.local\bin
```

#### 노트북 (사용자명: LMJ)
```
C:\Users\LMJ\AppData\Local\Microsoft\WinGet\Links
```

### 7-4. 설정 파일 동기화

`config.json` 파일은 환경 변수를 사용하므로 **동일한 내용**을 사용할 수 있습니다.

Git 저장소에 설정 파일을 포함하거나, 클라우드 동기화 도구로 관리하면 편리합니다.

---

## 8. 트러블슈팅

### 8-1. `uv --version`이 작동하지 않는 경우

**원인:** PATH 환경 변수가 제대로 설정되지 않음

**해결 방법:**
1. 경로가 정확한지 확인
2. 명령 프롬프트를 **새로 열었는지** 확인
3. 컴퓨터 재시작
4. 관리자 권한으로 명령 프롬프트 실행 후 재시도

### 8-2. MCP 서버가 연결되지 않는 경우

**원인 1:** 설정 파일 경로 오류

**해결 방법:**
```bash
# 설정 파일 위치 확인
echo %APPDATA%\Code\User\globalStorage\claude-code\config.json
```

**원인 2:** JSON 문법 오류

**해결 방법:**
- JSON 검증 도구로 `config.json` 확인
- 쉼표, 중괄호, 따옴표 등 문법 오류 수정

**원인 3:** Unity Editor가 실행되지 않음

**해결 방법:**
- Unity Editor를 먼저 실행한 후 Claude Code 사용

### 8-3. VSCode에서 Claude Code가 보이지 않는 경우

**해결 방법:**
1. VSCode 재시작
2. 확장 프로그램에서 Claude Code 활성화 확인
3. VSCode 업데이트 확인

### 8-4. Python 관련 오류

**해결 방법:**
```bash
# Python 버전 확인
python --version

# uv로 의존성 재설치
cd %LOCALAPPDATA%\UnityMCP\UnityMcpServer
uv sync --reinstall
```

### 8-5. 권한 오류 (Permission Denied)

**해결 방법:**
- 명령 프롬프트를 **관리자 권한**으로 실행
- 백신 프로그램이 차단하는지 확인

---

## 추가 팁

### Unity MCP로 할 수 있는 작업들

1. **씬 관리**
   - 씬 생성, 로드, 저장
   - GameObject 생성 및 조작

2. **스크립트 작성**
   - C# 스크립트 생성
   - 컴포넌트 추가/수정

3. **에셋 관리**
   - 머티리얼, 프리팹 생성
   - 에셋 검색 및 수정

4. **에디터 제어**
   - Play/Pause/Stop
   - 콘솔 로그 확인

### 유용한 Claude Code 명령어 예시

```
현재 씬에 큐브를 생성하고 빨간색 머티리얼을 적용해줘
```

```
PlayerController 스크립트를 수정해서 점프 기능을 추가해줘
```

```
Unity 콘솔의 에러 메시지를 확인하고 해결 방법을 알려줘
```

---

## 참고 자료

- [Unity MCP GitHub](https://github.com/coplaydev/UnityMcpServer)
- [Claude Code 공식 문서](https://docs.claude.com/claude-code)
- [MCP 프로토콜 문서](https://modelcontextprotocol.io/)
- [uv 공식 문서](https://docs.astral.sh/uv/)

---

## 라이선스 및 기여

이 가이드는 Novelian Magic Library Defense 프로젝트의 일부입니다.

문제가 발생하거나 개선 사항이 있다면 GitHub 이슈로 제보해주세요!
