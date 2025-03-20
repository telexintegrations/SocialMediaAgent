
---

# Social Media Content Generator

This project integrates Groq AI and Telex to generate social media posts based on a provided prompt (keyword). It allows users to input prompts in a Telex channel and automatically generate text-based social media posts, including platform-specific formatting, image generation, and content suggestions.

## Features

### Version 1.0 - Basic Social Media Post Generation with AI Support
- **Generate social media posts** based on a user-provided prompt (keyword).
- The content is generated using **Groq AI** and displayed in the Telex channel.

### Version 1.1 - AI Content Customization
- Users can **customize the tone**, **style**, **target audience**, and **post purpose** of the generated content to tailor it to their brand.

### Version 1.2 - Platform-Specific Formatting
- Automatically formats generated content for different platforms like **Twitter**, **Instagram**, **LinkedIn**, and **Facebook**.
- Previews the content to ensure it matches the platform’s requirements.

### Version 1.3 - Post Scheduling Within Telex
- Schedule AI-generated posts for **future publication** within Telex, allowing users to manage content in advance.

### Version 1.4 - AI Image Generation
- Generate **AI-generated images** alongside the text for social media posts to align with your company’s branding and style.

### Version 1.5 - Combined AI Content & Image Generation
- Generate **AI content** and the **corresponding image** together for a complete social media post.
- Users can preview the post before publishing.

### Version 1.6 - AI-Driven Content Suggestions
- Provides **AI-powered content recommendations** based on seasonality, current events, and trending topics to help users create relevant and engaging posts.

## Prerequisites

Before running the project, make sure you have the following:

- .NET 6 or higher
- Visual Studio or Visual Studio Code
- A valid **Groq API key** for content generation
- A **Telex Webhook URL** for sending messages to Telex channels

## Setup Instructions

### Clone the Repository
Clone the repository to your local machine using the following command:

```bash
git clone https://github.com/yourusername/social-media-content-generator.git
```

### Install Dependencies
Navigate to the project directory and run the following command to restore the required packages:

```bash
dotnet restore
```

### Configure Settings
1. Open the `appsettings.json` file.
2. Add your **Groq API key** and **Telex Settings with the Webhook URL**.

Example:
```json
{
  "GroqConfig": {
    "ApiKey": "your-groq-api-key",
    "ApiUrl": "https://api.groq.com/openai/v1/chat/completions"
  },
  "TelexConfig": {
    "data": {
      "target_url": "your-telex-webhook-url"
    }
  }
}
```

### Run the Application
After configuring the settings, you can run the application locally:

```bash
dotnet run
```

The application will start, and you can access it through `https://localhost:5001`.

## API Endpoints

### **Generate Post**
Generates a social media post based on the provided prompt.

**Endpoint**: `POST /generate-post`

**Request Body**:
```json
{
  "prompt": "Product Launch Announcement"
}
```

**Response**:
```json
{
  "postContent": "Here are some recent product launch announcements:..."
}
```

### **Send Message to Telex**
Sends the generated content to a specified Telex channel.

**Endpoint**: `POST /BingTelex`

**Request Body**:
```json
{
  "channelId": "your-channel-id",
  "promptRequest": {
    "prompt": "Product Launch Announcement"
  }
}
```

**Response**:
```json
{
  "status": "success",
  "message": "Social media content sent to Telex successfully"
}
```

## Contributing

We welcome contributions from the community! To contribute:

1. Fork the repository.
2. Create a new branch for your feature or fix.
3. Make your changes and test them.
4. Submit a pull request with a detailed description of the changes.

Please ensure that your code follows the existing style and conventions used in the project.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

### Additional Notes:
- **Customizable Features**: Users can provide specific settings such as tone, style, and platform type for more customized social media posts.
- **Error Handling**: The system handles errors such as empty prompts or failed content generation and informs users accordingly.
- **Preview Mode**: Users can preview generated posts before sending them to Telex, ensuring the final product meets their needs.
---