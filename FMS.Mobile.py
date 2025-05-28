import os

# MAUI 项目根目录
root_dir = rf"D:\Programing\C#\FMS\FMS.Mobile"
output_file = rf"D:\Programing\C#\FMS\_FMS.MobileCode.txt"

# 根目录允许的文件扩展
target_exts = [".cs", ".xaml", ".json", ".xml", ".plist", ".svg", ".csproj"]

# 子目录中允许遍历的目标文件夹
target_folders = [
    "Converters",
    "Models",
    "Platforms",
    "Properties",
    "Resources",
    "Services",
    "ViewModels",
    "Views",
    "Controls",
]

all_paths = []  # 记录所有相对路径


with open(output_file, "w", encoding="utf-8") as out:
    for dirpath, _, filenames in os.walk(root_dir):
        rel_dir = os.path.relpath(dirpath, root_dir)

        # 根目录允许写所有 target_exts
        if rel_dir != "." and not any(rel_dir.startswith(f) for f in target_folders):
            continue

        for filename in filenames:
            ext = os.path.splitext(filename)[1].lower()

            # 根目录允许所有 target_exts
            # 子目录仅输出 .cs/.xaml/.xml/.json 等源码/配置文件
            if (rel_dir == "." and ext in target_exts) or (
                rel_dir != "." and ext in target_exts
            ):

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
