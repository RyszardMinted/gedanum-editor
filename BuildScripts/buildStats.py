import matplotlib.pyplot as plt
import csv

# Parse Build Steps Data
steps = []
durations = []

with open("../BuildData.csv", "r") as file:
    reader = csv.reader(file)
    for row in reader:
        if not row:
            continue
        if row[0] == "Step" and row[1] == "Duration (s)":
            data_section = "steps"
            continue
        elif row[0] == "Asset" and row[1] == "Size (KB)":
            data_section = "assets"
            continue
        if data_section == "steps" and len(row) == 2:
            steps.append(row[0])
            durations.append(float(row[1]))

    # Shorten labels if necessary
shortened_steps = [step[:55] + "..." if len(step) > 55 else step for step in steps]

# Highlight the longest step
colors = ['skyblue' if duration < max(durations) else 'orange' for duration in durations]

# Plot Horizontal Bar Chart
plt.figure(figsize=(12, 6))
bars = plt.barh(shortened_steps, durations, color=colors)
plt.xlabel("Duration (s)")
plt.ylabel("Build Steps")
plt.title("Build Step Durations")
plt.tight_layout()

# Add data labels
for bar in bars:
    width = bar.get_width()
    plt.text(
        width + 0.5,  # Slightly beyond the bar's end
        bar.get_y() + bar.get_height() / 2,
        f"{width:.2f}s",
        va="center",
        fontsize=8
    )

plt.savefig("results/build_step_durations_horizontal.png")
# plt.show()

assets = []
sizes = []
data_section = "none"  # Tracks which section of the CSV we are in

with open("../BuildData.csv", "r") as file:
    reader = csv.reader(file)
    for row in reader:
        if not row:
            continue  # Skip empty rows
        if row[0] == "Step" and row[1] == "Duration (s)":
            data_section = "steps"
            continue
        elif row[0] == "Asset" and row[1] == "Size (KB)":
            data_section = "assets"
            continue
        if data_section == "assets" and len(row) == 2:
            assets.append(row[0])
            sizes.append(float(row[1]) / 1024.0)  # Convert KB to MB

    # Combine Very Small Elements into "Other"
other_threshold = 0.056  # Files smaller than 56 KB (0.056 MB)
aggregated_assets = []
aggregated_sizes = []
other_size = 0

for asset, size in zip(assets, sizes):
    if size >= other_threshold:
        aggregated_assets.append(asset)
        aggregated_sizes.append(size)
    else:
        other_size += size

# Add "Other" category if applicable
if other_size > 0:
    aggregated_assets.append("Other")
    aggregated_sizes.append(other_size)

# Sort assets by size
sorted_data = sorted(zip(aggregated_assets, aggregated_sizes), key=lambda x: x[1], reverse=True)
aggregated_assets, aggregated_sizes = zip(*sorted_data)

# Create the Bar Chart
plt.figure(figsize=(12, 8))
bars = plt.barh(aggregated_assets, aggregated_sizes, color="skyblue")
plt.xlabel("Size (MB)", fontsize=12)
plt.ylabel("Assets", fontsize=12)
plt.title("Asset Contributions to Build Size", fontsize=14)

# Add data labels to each bar
for bar in bars:
    width = bar.get_width()
    plt.text(
        width + 0.05,  # Slightly beyond the bar's end
        bar.get_y() + bar.get_height() / 2,  # Centered vertically on the bar
        f"{width:.2f} MB",  # Format the size value
        va="center",
        fontsize=10,
    )

plt.tight_layout()
plt.savefig("results/asset_sizes_bar_chart_mb.png")
# plt.show()