import os

root_dir = rf"D:\Programing\C#\FMS\FMS.Server" 
output_file = f"D:\Programing\C#\FMS\_FMS.ServerCode.txt"

# 选取的源代码/配置/重要脚本文件类型
target_exts = [".cs", ".json", ".http", ".csproj"]
# 只保留这些目录下的 .cs 文件
target_folders = ["Controllers", "Data", "Models", "Migrations","Converter"]

all_paths = []  # 记录所有相对路径


with open(output_file, "w", encoding="utf-8") as out:
    for dirpath, _, filenames in os.walk(root_dir):
        # 筛选只遍历目标目录，或者根目录
        rel_dir = os.path.relpath(dirpath, root_dir)
        if rel_dir != "." and not any(rel_dir.startswith(f) for f in target_folders):
            continue
        for filename in filenames:
            ext = os.path.splitext(filename)[1].lower()
            # 根目录允许所有指定类型；目标子目录只允许 .cs 文件
            if ((rel_dir == "." and ext in target_exts) or
                (rel_dir != "." and ext == ".cs")):
                file_path = os.path.join(dirpath, filename)
                rel_path = os.path.relpath(file_path, root_dir)
                all_paths.append(rel_path)
                out.write(f"\n\n# --- {rel_path} ---\n")
                with open(file_path, "r", encoding="utf-8", errors="ignore") as f:
                    out.write(f.read())
                    
                    
    # 写入文件索引列表到文件头
    out.seek(0, 0)
    out.write("### 文件索引\n")
    out.write("\n".join(all_paths))
    out.write("\n\n### 文件内容\n")
    
print(f"已合并导出到 {output_file}")
