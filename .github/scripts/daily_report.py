import os
import json
import requests
from datetime import datetime, timedelta

print("=== Daily Report Generation Started ===")

# LMJ: Load environment variables
notion_token = os.environ["NOTION_TOKEN"]
report_page_id = os.environ["NOTION_REPORT_PAGE_ID"]
gemini_api_key = os.environ["GEMINI_API_KEY"]
github_token = os.environ["GITHUB_TOKEN"]
repo = os.environ["GITHUB_REPOSITORY"]

# LMJ: Calculate yesterday's date
now = datetime.utcnow()
yesterday = now - timedelta(days=1)
yesterday_start = yesterday.replace(hour=0, minute=0, second=0, microsecond=0)
yesterday_end = yesterday.replace(hour=23, minute=59, second=59, microsecond=999999)

print(f"Collecting issues from {yesterday_start} to {yesterday_end}")

# LMJ: Fetch issues from GitHub
headers = {
    "Authorization": f"token {github_token}",
    "Accept": "application/vnd.github.v3+json"
}

issues_url = f"https://api.github.com/repos/{repo}/issues"
params = {
    "state": "all",
    "since": yesterday_start.isoformat() + "Z",
    "per_page": 100
}

response = requests.get(issues_url, headers=headers, params=params)
if response.status_code != 200:
    print(f"❌ GitHub API error: {response.status_code}")
    exit(1)

all_issues = response.json()

# LMJ: Filter issues created yesterday
yesterday_issues = []
for issue in all_issues:
    created_at = datetime.strptime(issue["created_at"], "%Y-%m-%dT%H:%M:%SZ")
    if yesterday_start <= created_at <= yesterday_end:
        # Skip pull requests
        if "pull_request" not in issue:
            yesterday_issues.append(issue)

print(f"Found {len(yesterday_issues)} issues from yesterday")

if len(yesterday_issues) == 0:
    print("No issues to report")
    exit(0)

# LMJ: Prepare issue list for Gemini
issues_text = ""
for idx, issue in enumerate(yesterday_issues, 1):
    issues_text += f"{idx}. #{issue['number']} - {issue['title']}\n"
    issues_text += f"   상태: {issue['state']}\n"
    labels = [label['name'] for label in issue.get('labels', [])]
    if labels:
        issues_text += f"   태그: {', '.join(labels)}\n"
    issues_text += f"   링크: {issue['html_url']}\n\n"

# LMJ: Generate summary using Gemini
gemini_url = f"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={gemini_api_key}"

prompt = f"""다음은 어제({yesterday.strftime('%Y년 %m월 %d일')}) 등록된 GitHub Issue 목록입니다.

{issues_text}

이 이슈들을 분석하여 다음 형식의 일간 보고서를 작성해주세요:

## 📊 요약
- 총 이슈 수: {len(yesterday_issues)}개
- 주요 카테고리별 분류

## 🔍 주요 이슈
각 이슈를 간단히 요약 (1-2문장)

## 💡 종합 의견
전체적인 진행 상황과 주목할 점

한국어로 작성하고, 전문적이고 간결하게 작성해주세요."""

gemini_payload = {
    "contents": [{
        "parts": [{
            "text": prompt
        }]
    }]
}

try:
    response = requests.post(gemini_url, json=gemini_payload)
    if response.status_code == 200:
        result = response.json()
        summary = result["candidates"][0]["content"]["parts"][0]["text"]
        print("✅ Gemini summary generated")
    else:
        print(f"❌ Gemini API error: {response.status_code} - {response.text}")
        summary = f"총 {len(yesterday_issues)}개의 이슈가 등록되었습니다."
except Exception as e:
    print(f"❌ Gemini error: {e}")
    summary = f"총 {len(yesterday_issues)}개의 이슈가 등록되었습니다."

# LMJ: Create Notion page
notion_headers = {
    "Authorization": f"Bearer {notion_token}",
    "Notion-Version": "2022-06-28",
    "Content-Type": "application/json"
}

page_title = f"{yesterday.strftime('%Y년 %m월 %d일')} 개발 일간 보고"

# LMJ: Build page content
children = [
    {
        "object": "block",
        "type": "heading_2",
        "heading_2": {
            "rich_text": [{"type": "text", "text": {"content": "AI 요약"}}]
        }
    },
    {
        "object": "block",
        "type": "paragraph",
        "paragraph": {
            "rich_text": [{"type": "text", "text": {"content": summary}}]
        }
    },
    {
        "object": "block",
        "type": "divider",
        "divider": {}
    },
    {
        "object": "block",
        "type": "heading_2",
        "heading_2": {
            "rich_text": [{"type": "text", "text": {"content": "상세 이슈 목록"}}]
        }
    }
]

# LMJ: Add each issue
for issue in yesterday_issues:
    children.append({
        "object": "block",
        "type": "heading_3",
        "heading_3": {
            "rich_text": [{
                "type": "text",
                "text": {"content": f"#{issue['number']} {issue['title']}"},
                "annotations": {"bold": True}
            }]
        }
    })
    
    children.append({
        "object": "block",
        "type": "paragraph",
        "paragraph": {
            "rich_text": [
                {"type": "text", "text": {"content": f"상태: {issue['state']} | "}},
                {"type": "text", "text": {"content": "링크", "link": {"url": issue['html_url']}}}
            ]
        }
    })

# LMJ: Create page
create_page_url = "https://api.notion.com/v1/pages"
page_data = {
    "parent": {"page_id": report_page_id},
    "properties": {
        "title": {
            "title": [{"text": {"content": page_title}}]
        }
    },
    "children": children
}

try:
    response = requests.post(create_page_url, headers=notion_headers, json=page_data)
    if response.status_code == 200:
        page_url = response.json()["url"]
        print(f"✅ Daily report created: {page_url}")
    else:
        print(f"❌ Notion API error: {response.status_code} - {response.text}")
        exit(1)
except Exception as e:
    print(f"❌ Error creating Notion page: {e}")
    exit(1)

print("=== Daily Report Generation Completed ===")