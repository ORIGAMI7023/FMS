import os
import json
import requests

# 服务器地址
url = "http://localhost:7050/api/revenue/import"

# 获取当前脚本所在目录
base_dir = os.path.dirname(__file__)
json_path = os.path.join(base_dir, "RevenueRecords_2024.json")

# 读取 JSON 数据
with open(json_path, "r", encoding="utf-8") as f:
    data = json.load(f)

# 发送 POST 请求
response = requests.post(url, json=data, headers={"Content-Type": "application/json"}, verify=False)

# 输出响应结果
print("状态码:", response.status_code)
print("响应内容:", response.text)
