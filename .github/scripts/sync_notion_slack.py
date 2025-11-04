import os
import json
import requests
from notion_client import Client

notion = Client(auth=os.environ["NOTION_TOKEN"])
database_id = os.environ["NOTION_DATABASE_ID"]
slack_webhook = os.environ["SLACK_WEBHOOK_URL"]
event = json.loads(os.environ["GITHUB_EVENT"])
repo = os.environ["GITHUB_REPOSITORY"]

# LMJ: Determine if it's an issue or PR
if "issue" in event and "pull_request" not in event["issue"]:
    item_type = "Issue"
    item = event["issue"]
    action_emoji = "🐛"
elif "pull_request" in event:
    item_type = "PR"
    item = event["pull_request"]
    action_emoji = "🔀"
else:
    print("Not an issue or PR, skipping")
    exit(0)

# LMJ: Extract data
title = item["title"]
body = item["body"] or "No description"
state = item["state"]
url = item["html_url"]
labels = [label["name"] for label in item.get("labels", [])]
assignees = [assignee["login"] for assignee in item.get("assignees", [])]
number = item["number"]
action = event["action"]

# LMJ: Map status
status_map = {
    "open": "진행중",
    "closed": "완료",
    "reopened": "재오픈"
}

action_map = {
    "opened": "생성됨",
    "edited": "수정됨",
    "closed": "완료됨",
    "reopened": "재오픈됨"
}

# LMJ: Create or update Notion page
notion_url = None
try:
    # LMJ: Check if page already exists
    existing_pages = notion.databases.query(
        database_id=database_id,
        filter={
            "property": "제목",
            "title": {"contains": f"#{number}"}
        }
    )
    
    if existing_pages["results"] and action != "opened":
        # LMJ: Update existing page
        page_id = existing_pages["results"][0]["id"]
        notion.pages.update(
            page_id=page_id,
            properties={
                "상태": {"select": {"name": status_map.get(state, "진행중")}}
            }
        )
        notion_url = f"https://notion.so/{page_id.replace('-', '')}"
        print(f"Updated {item_type} #{number} in Notion")
    else:
        # LMJ: Create new page
        response = notion.pages.create(
            parent={"database_id": database_id},
            properties={
                "제목": {
                    "title": [{"text": {"content": f"[{item_type} #{number}] {title}"}}]
                },
                "타입": {"select": {"name": item_type}},
                "상태": {"select": {"name": status_map.get(state, "진행중")}},
                "태그": {"multi_select": [{"name": label} for label in labels]},
                "GitHub URL": {"url": url}
            },
            children=[
                {
                    "object": "block",
                    "type": "paragraph",
                    "paragraph": {
                        "rich_text": [{"text": {"content": body[:2000]}}]
                    }
                }
            ]
        )
        notion_url = f"https://notion.so/{response['id'].replace('-', '')}"
        print(f"Created {item_type} #{number} in Notion")
    
except Exception as e:
    print(f"Notion Error: {e}")
    notion_url = "Failed to sync"

# LMJ: Send Slack notification
try:
    # LMJ: Build label text
    label_text = ", ".join([f"`{label}`" for label in labels]) if labels else "없음"
    
    # LMJ: Build assignee text
    assignee_text = ", ".join([f"@{a}" for a in assignees]) if assignees else "없음"
    
    # LMJ: Color based on action
    color_map = {
        "opened": "#36a64f",    # green
        "closed": "#808080",    # gray
        "edited": "#2196F3",    # blue
        "reopened": "#ff9800"   # orange
    }
    
    slack_payload = {
        "attachments": [
            {
                "color": color_map.get(action, "#808080"),
                "blocks": [
                    {
                        "type": "header",
                        "text": {
                            "type": "plain_text",
                            "text": f"{action_emoji} {item_type} #{number} {action_map.get(action, action)}"
                        }
                    },
                    {
                        "type": "section",
                        "text": {
                            "type": "mrkdwn",
                            "text": f"*{title}*"
                        }
                    },
                    {
                        "type": "section",
                        "fields": [
                            {
                                "type": "mrkdwn",
                                "text": f"*Repository:*\n{repo}"
                            },
                            {
                                "type": "mrkdwn",
                                "text": f"*상태:*\n{status_map.get(state, state)}"
                            },
                            {
                                "type": "mrkdwn",
                                "text": f"*담당자:*\n{assignee_text}"
                            },
                            {
                                "type": "mrkdwn",
                                "text": f"*태그:*\n{label_text}"
                            }
                        ]
                    },
                    {
                        "type": "section",
                        "text": {
                            "type": "mrkdwn",
                            "text": f"```{body[:300]}...```" if len(body) > 300 else f"```{body}```"
                        }
                    },
                    {
                        "type": "actions",
                        "elements": [
                            {
                                "type": "button",
                                "text": {
                                    "type": "plain_text",
                                    "text": "GitHub에서 보기"
                                },
                                "url": url,
                                "style": "primary"
                            },
                            {
                                "type": "button",
                                "text": {
                                    "type": "plain_text",
                                    "text": "Notion에서 보기"
                                },
                                "url": notion_url
                            }
                        ]
                    }
                ]
            }
        ]
    }
    
    response = requests.post(slack_webhook, json=slack_payload)
    if response.status_code == 200:
        print(f"Sent Slack notification for {item_type} #{number}")
    else:
        print(f"Slack Error: {response.status_code} - {response.text}")
        
except Exception as e:
    print(f"Slack Error: {e}")

print("Sync completed successfully")