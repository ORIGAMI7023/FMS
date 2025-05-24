import subprocess
import os

scripts = [
    "D:\Programing\C#\FMS\FMS.Server.py",
    "D:\Programing\C#\FMS\FMS.Client.py",
    "D:\Programing\C#\FMS\FMS.Mobile.py"
]

for script in scripts:
    print(f"\n📦 正在运行：{script}")
    result = subprocess.run(["python", script], capture_output=True, text=True)

    if result.returncode == 0:
        print(f"✅ {script} 运行成功")
    else:
        print(f"❌ {script} 运行失败：\n{result.stderr}")

print("\n🎉 所有导出任务已完成")
