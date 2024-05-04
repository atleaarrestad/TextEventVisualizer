# TextEventVisualizer

TextEventVisualizer is a tool designed to process news articles and produce chronological timelines for important events. 

The ever-growing volume of text data, particularly news articles, present both an
opportunity and a challenge. While it provides access to a lot of information, navigating
through it and extracting important facts can be overwhelming. This tool aims to
address this challenge by automatically recognizing and organizing
important events from large text corpora, and present them on a user-friendly timeline.

The tool has been developed using news articles from 2020 in the category world news. The articles have been collected from huffpost.com.


Link to website running the tool: https://72b0-2a01-799-5a-8f00-4485-936a-7166-b698.ngrok-free.app/

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
     3. Skip to step D and E. If you get an error or it is unsuccessful run `curl -fsSL https://nvidia.github.io/libnvidia-container/gpgkey | sudo gpg --dearmor -o /usr/share/keyrings/nvidia-container-toolkit-keyring.gpg \
  && curl -s -L https://nvidia.github.io/libnvidia-container/stable/deb/nvidia-container-toolkit.list | \
    sed 's#deb https://#deb [signed-by=/usr/share/keyrings/nvidia-container-toolkit-keyring.gpg] https://#g' | \
    sudo tee /etc/apt/sources.list.d/nvidia-container-toolkit.list` to configure the production repository and try step D and E again.
     4. Run `sudo apt-get update` to update your package lists.
     5. Run `sudo apt-get install -y nvidia-container-toolkit` to install the NVIDIA container toolkit.
     6. Now, you should be able to run `nvidia-smi` and get information about your GPU, indicating a successful setup.
     7. Check if Docker is using WSL 2. Go to Docker -> Settings -> Resources. Ensure the WSL 2 backend is enabled.
     8. Exit the Ubuntu terminal.
     9. Run the `start_with_GPU` file in the root project folder.
     10. After all containers are up and running. Go into the ollama container.
     11. Go to "Exec" and run `ollama pull llama2`.
   
   - Subsequent startups after doing this once only requires you to run the `start_with_GPU` file in the root project folder. 

4. **Start the aplication**
   - Run the application from your preferred IDE.

---
