# TextEventVisualizer

TextEventVisualizer is a tool designed to process news articles and produce chronological timelines for important events.

---

## Developer Setup

Follow these steps to set up the TextEventVisualizer environment on your local machine:

1. **Install .NET 8:**

   - Download and install .NET 8 from the official [.NET Download Page](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

2. **Download the Dataset:**

   - Download the required dataset from Kaggle: [News Category Dataset](https://www.kaggle.com/datasets/rmisra/news-category-dataset).
   - After downloading, place the `.json` file in the `TextEventVisualizer/Data` folder.
   - rename the .json file to `news_articles.json`

3. **Docker Setup:**
   - First, download and install Docker from the [Docker download page](https://www.docker.com/products/docker-desktop/).

   - **CPU Setup**
     1. Simply run the `start_with_CPU` file in the root project folder. Running on the CPU has no extra requirements, but is slower.

   - **GPU Setup**
     - Refer to [Nvidia WSL2](https://docs.nvidia.com/cuda/wsl-user-guide/index.html) for limitations and troubleshooting.
     1. Run `wsl --install` in a terminal.
     2. The terminal should now be in Ubuntu. If not, type `wsl -d Ubuntu`.
     3. Run `sudo apt-get update` to update your package lists.
     4. Run `sudo apt-get install -y nvidia-container-toolkit` to install the NVIDIA container toolkit.
     5. ~~Run `sudo apt install -y nvidia-driver-510`~~ (replace `510` with the latest or required version for your system). This step is probably not required according to [Nvidia WSL2](https://docs.nvidia.com/cuda/wsl-user-guide/index.html).
     6. Now, you should be able to run `nvidia-smi` and get information about your GPU, indicating a successful setup.
     7. Check if Docker is using WSL 2. Go to Docker -> Settings -> Resources. Ensure the WSL 2 backend is enabled.
     8. Exit the Ubuntu terminal.
     9. Run the `start_with_GPU` file in the root project folder.


---
