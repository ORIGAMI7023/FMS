import pandas as pd
import json
import os

base_dir = os.path.dirname(os.path.abspath(__file__))  # 获取当前脚本的绝对路径
xlsx_path = os.path.join(base_dir, "2024简单结构汇总.xlsx")

# 读取 Excel（无列头）
df = pd.read_excel(xlsx_path, header=None)

# 表头处理
header_names = df.iloc[0].fillna("")
item_types = df.iloc[1].fillna("")
df_data = df.iloc[2:].reset_index(drop=True)

records = []

for i in range(len(df_data)):
    row = df_data.iloc[i]
    raw_date = row[0]

    try:
        date_obj = pd.to_datetime(raw_date, format="%Y/%m/%d", errors="coerce")
    except:
        continue

    if pd.isna(date_obj):
        continue

    date_str = date_obj.strftime("%Y-%m-%d")

    for col_idx in range(2, len(row)):  # B列跳过
        value = row[col_idx]
        if pd.isna(value) or not isinstance(value, (int, float)):
            continue

        owner = str(header_names[col_idx]).strip()
        item_type = str(item_types[col_idx]).strip()

        if not item_type:
            continue

        # W列 = 22（零药跳过）
        if col_idx == 22:
            continue

        is_visit_count = (col_idx >= 23)
        is_excluded = owner in ["护士站", "检验"]

        record = {
            "date": date_str,
            "owner": owner,
            "itemType": item_type,
            "value": round(float(value), 2),
            "isVisitCount": is_visit_count,
            "isExcludedFromSummary": is_excluded,
            "source": "Admin"
        }

        records.append(record)

# 导出 JSON 文件import os
output_path = os.path.join(os.path.dirname(__file__), "RevenueRecords_2024.json")
with open(output_path, "w", encoding="utf-8") as f:

    json.dump(records, f, indent=2, ensure_ascii=False)

print(f"✅ 导出成功，共 {len(records)} 条记录。输出文件：RevenueRecords_2024.json")
