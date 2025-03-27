---

# 📢 **Social Media Content Generator**

AI-powered tool to generate and post social media content directly to Telex channels. The system integrates **Groq AI** for content generation and **Telex** for content distribution, enabling businesses to automate the process of creating engaging social media posts.

---

## 🚀 **Overview**

The **Social Media Content Generator** automates the process of generating social media posts based on user-defined keywords or prompts. This integration uses **Groq AI** for content generation and sends the results to a **Telex channel**. The tool allows users to generate, customize, and post content to various platforms, streamlining social media management.

---

## ✨ **Features**

✅ **Groq AI Integration** - Automatically generates social media content based on a user-provided prompt (e.g., "Product Launch Announcement").  
✅ **Telex Channel Integration** - Sends the generated content to a Telex channel for easy sharing and management.  
✅ **Customizable AI Content** - Users can specify the tone, style, audience, and purpose for more tailored social media posts.  
✅ **Content Preview** - A preview of generated posts is displayed in the Telex channel before posting.  
✅ **Error Handling** - The system handles errors like empty or invalid prompts and notifies users accordingly.  

---

## 📌 **Tech Stack**

- **Backend**: ASP.NET Core, C#
- **API & Integration**: Groq API, Telex API
- **HTTP Client**: `HttpClient` for external requests
- **Configuration**: `appsettings.json` for app settings and API configurations

---

## 🛠 **Installation Guide**

### 1️⃣ **Clone the Repository**

```bash
git clone https://github.com/your-username/social-media-content-generator.git
cd social-media-content-generator
```

### 2️⃣ **Install Dependencies**

Restore and install the required dependencies:

```bash
dotnet restore
```

### 3️⃣ **Set Up AppSettings**

In the `appsettings.json` file, add the following sections for **Groq API** and **Telex** integration settings:

```json
{
  "GroqConfig": {
    "ApiKey": "your_groq_api_key", // Add your Groq API key here
    "ApiUrl": "https://api.groq.com/openai/v1/chat/completions" // The Groq API URL
  },
  "TelexConfig": {
    "data": {
      "target_url": "your_telex_webhook_url" // Add your Telex webhook URL here
    }
  }
}
```

- Replace `"your_groq_api_key"` with your actual Groq API key.
- Replace `"your_telex_webhook_url"` with the **Telex webhook URL** where the generated content will be sent.

### 4️⃣ **Telex Integration Setup**

To complete the **Telex** integration:

- Add the **URL of your deployed app** (which will contain the `integration.json`) to the **Telex channel** configuration.
- Ensure that the **Webhook URL** from Telex is added to the `target_url` in the `TelexConfig` section of your `appsettings.json`.
- Specify the **platform** you want to post your generated content from the AI directly in the request or configuration.

### 5️⃣ **Start the Application**

Run the following command to start the application:

```bash
dotnet run
```

---

## 👨‍💻 **Usage**

- **Generate Social Media Post**: To generate a post, send a request with a prompt (e.g., `/generate-post "Product Launch Announcement"`) to the Telex channel.
- **Schedule Posts**: You can schedule posts for future publication within the Telex channel using specific commands and set times.
- **Customize Content**: Specify tone, style, audience, and post purpose to generate customized content for your social media.

---

## 📂 **Folder Structure**

```
├── Controllers
│   ├── TelexController.cs
│   └── GroqController.cs
├── Models
│   ├── Request
│   │   └── GroqPromptRequest.cs
│   ├── Response
│   │   └── TelexMessageResponse.cs
├── Repositories
│   ├── Interfaces
│   │   └── ITelexService.cs
│   └── Implementation
│       └── TelexRepository.cs
├── Services
│   └── GroqService.cs
└── appsettings.json
```

---

## 🔧 **Contributing**

1. **Fork the Repository**  
   Fork the project repository to your own GitHub account.

2. **Create a Branch**  
   Create a new branch for your feature or bugfix:

   ```bash
   git checkout -b feature/branch-name
   ```

3. **Commit Changes**  
   Commit your changes with descriptive messages:

   ```bash
   git commit -m "Added new feature"
   ```

4. **Push Changes**  
   Push the changes to your forked repository:

   ```bash
   git push origin feature/branch-name
   ```

5. **Create a Pull Request**  
   Open a pull request (PR) to the original repository from your fork.

---

## 🙋‍♂️ **Questions or Issues?**

Feel free to reach out with any questions or open an issue if something isn't working as expected.

---

### Notes:
- This README outlines the setup and integration steps for a project that automatically generates and posts social media content using **Groq AI** and **Telex** integration.
- It includes instructions for setting up the **Groq API** and **Telex Webhook**, which are crucial for the system to work correctly.