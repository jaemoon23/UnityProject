import os
import json
import requests

print("=== Slack Notification Started ===")

# LMJ: Load environment variables
slack_webhook = os.environ.get("SLACK_WEBHOOK_URL")
mapping_json = os.environ.get("GITHUB_SLACK_MAPPING", "{}")
event = json.loads(os.environ["GITHUB_EVENT"])
repo = os.environ["GITHUB_REPOSITORY"]

# LMJ: Parse GitHub-Slack mapping
try:
    github_slack_map = json.loads(mapping_json)
except:
    github_slack_map = {}

# LMJ: Determine if it's an issue or PR
if "issue" in event and "pull_request" not in event["issue"]:
    item_type = "Issue"
    item = event["issue"]
    emoji = "🐛"
elif "pull_request" in event:
    item_type = "PR"
    item = event["pull_request"]
    emoji = "🔀"
else:
    print("Not an issue or PR, skipping")
    exit(0)

# LMJ: Extract data
title = item["title"]
body = item.get("body", "No description")[:300]
state = item["state"]
url = item["html_url"]
number = item["number"]
action = event["action"]

# LMJ: Get assignees and convert to Slack mentions
assignees = item.get("assignees", [])
slack_mentions = []
for assignee in assignees:
    github_username = assignee["login"]
    slack_id = github_slack_map.get(github_username)
    if slack_id:
        slack_mentions.append(f"<@{slack_id}>")
    else:
        slack_mentions.append(f"@{github_username}")

mention_text = ", ".join(slack_mentions) if slack_mentions else "담당자 없음"

# LMJ: Action text
action_map = {
    "opened": "생성됨",
    "closed": "완료됨",
    "reopened": "재오픈됨"
}
action_text = action_map.get(action, action)

# LMJ: Build Slack message
message_text = f"{emoji} *{item_type} #{number} {action_text}*\n\n"
message_text += f"*{title}*\n\n"
message_text += f"📝 {body}\n\n"
message_text += f"👤 담당자: {mention_text}\n"
message_text += f"🔗 {url}"

# LMJ: Add mention at the beginning if assignees exist
if slack_mentions:
    full_message = f"{' '.join(slack_mentions)}\n\n{message_text}"
else:
    full_message = message_text

slack_payload = {
    "text": full_message
}

# LMJ: Send to Slack
try:
    response = requests.post(slack_webhook, json=slack_payload)
    if response.status_code == 200:
        print(f"✅ Slack notification sent for {item_type} #{number}")
    else:
        print(f"❌ Slack error: {response.status_code} - {response.text}")
        exit(1)
except Exception as e:
    print(f"❌ Error: {e}")
    exit(1)

print("=== Slack Notification Completed ===")