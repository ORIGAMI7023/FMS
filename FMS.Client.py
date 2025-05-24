import os

# 客户端项目根目录
root_dir = rf"D:\Programing\C#\FMS\FMS.Client"
output_file = rf"D:\Programing\C#\FMS\_FMS.ClientCode.txt"

# 根目录允许导出的文件类型
target_exts = [".cs", ".xaml", ".json", ".csproj"]

# 限定自动扫描的子目录（保持项目结构清晰）
target_folders = [
    "Models", "ViewModels", "Views", "Services", "Config"
]

with open(output_file, "w", encoding="utf-8") as out:
    for dirpath, _, filenames in os.walk(root_dir):
        rel_dir = os.path.relpath(dirpath, root_dir)

        # 根目录允许写全部扩展名；子目录需属于指定结构
        if rel_dir != "." and not any(rel_dir.startswith(f) for f in target_folders):
            continue

        for filename in filenames:
            ext = os.path.splitext(filename)[1].lower()

            if ((rel_dir == "." and ext in target_exts) or
                (rel_dir != "." and ext in target_exts)):

                file_path = os.path.join(dirpath, filename)
                rel_path = os.path.relpath(file_path, root_dir)
                out.write(f"\n\n# --- {rel_path} ---\n")
                with open(file_path, "r", encoding="utf-8", errors="ignore") as f:
                    out.write(f.read())

print(f"FMS.Client 已合并导出到 {output_file}")