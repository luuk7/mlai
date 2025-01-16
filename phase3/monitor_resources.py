import argparse
import psutil
import time
import csv
import os
from datetime import datetime
from pynvml import nvmlInit, nvmlDeviceGetHandleByIndex, nvmlDeviceGetMemoryInfo, nvmlDeviceGetUtilizationRates, nvmlDeviceGetCount

def get_gpu_metrics():
    """Get GPU and VRAM usage for all GPUs."""
    nvmlInit()
    gpu_usage = 0
    vram_usage = 0
    device_count = nvmlDeviceGetCount()

    if device_count == 0:
        return -1, -1

    for i in range(device_count):
        handle = nvmlDeviceGetHandleByIndex(i)
        utilization = nvmlDeviceGetUtilizationRates(handle)
        memory_info = nvmlDeviceGetMemoryInfo(handle)

        gpu_usage += utilization.gpu
        vram_usage += memory_info.used / (1024 ** 2)  # Convert bytes to MB

    return gpu_usage, vram_usage

def monitor_processes(output_csv, run_id, interval=5):
    """Monitor processes and save metrics to a CSV file."""
    with open(output_csv, mode='w', newline='') as file:
        writer = csv.writer(file)
        writer.writerow(["Run ID", "Timestamp", "CPU Usage (%)", "RAM Usage (MB)", "GPU Usage (%)", "VRAM Usage (MB)"])

        while True:
            total_cpu = 0
            total_ram = 0

            for proc in psutil.process_iter(['name', 'cpu_percent', 'memory_info']):
                try:
                    if "python" in proc.info['name'].lower() or "unityenvironment" in proc.info['name'].lower():
                        total_cpu += proc.info['cpu_percent']
                        total_ram += proc.info['memory_info'].rss / (1024 ** 2)  # Convert bytes to MB
                except (psutil.NoSuchProcess, psutil.AccessDenied, psutil.ZombieProcess):
                    continue

            gpu_usage, vram_usage = get_gpu_metrics()

            timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
            writer.writerow([run_id, timestamp, total_cpu, total_ram, gpu_usage, vram_usage])
            print(f"{timestamp} | Run ID: {run_id} | CPU: {total_cpu:.2f}% | RAM: {total_ram:.2f} MB | GPU: {gpu_usage:.2f}% | VRAM: {vram_usage:.2f} MB")

            time.sleep(interval)

if __name__ == "__main__":

    # Create the parser
    parser = argparse.ArgumentParser(description="Collects the data")

    # Add the --myvariable argument
    parser.add_argument("--run-id", type=str, required=True, help="This should be the same as the --run-id arguments you passed to mlagents-learn.")

    # Parse the arguments
    args = parser.parse_args()

    output_file = f"results/{args.run_id}/resource_metrics.csv"
    monitoring_interval = 1 # in seconds

    # Check if file exists and prompt before overwriting
    if os.path.exists(output_file):
        response = input(f"File {output_file} already exists. Do you want to overwrite it? (y/n): ").strip().lower()
        if response != 'y':
            print("Operation cancelled.")
            exit()

    print(f"Data will be saved to {output_file}")

    proc_count = 0
    print("Processes found:")
    for proc in psutil.process_iter(['name', 'cpu_percent', 'memory_info']):
        try:
            if "python" in proc.info['name'].lower() or "unityenvironment" in proc.info['name'].lower():
                print(proc.info['name'])
                proc_count += 1
        except (psutil.NoSuchProcess, psutil.AccessDenied, psutil.ZombieProcess):
            continue
    print(f"{proc_count} processes in total.")

    monitor_processes(output_file, args.run_id, monitoring_interval)
