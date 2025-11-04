# Simple Copilot Chat Frontend

A React-based chat application for interacting with an AI assistant (backend), designed for deployment on Azure Container Apps.

## Getting Started

### Prerequisites

- **Node.js** (v16 or higher)
- **npm** (v7 or higher)
- **Git**

### Installation

1. **Install Dependencies:**

   ```bash
   npm install
   ```

1. **Set Environment Variables:**

   Create a `.env` file in the root directory of frontend and add the following environment variables:

   ```bash
   API_URL=https://your-backend-api-url/chat
   REACT_APP_PROXY_URL=/api/chat
   PORT=80
   ```

   **Note:** 
   - `API_URL` is used by the Node.js server (`server.js`) at runtime to proxy requests to the backend API
   - `REACT_APP_PROXY_URL` is baked into the React build - use the relative path `/api/chat` so it works both locally and when deployed
   - For local development, set `API_URL=http://localhost:5000/chat`
   - When deployed to Azure, `API_URL` is automatically set via Container Apps environment variables

1. **Run the Application:**

   ```bash
   npm start
   ```

   The application will be available at `http://localhost:80`.

### Docker Usage (Optional)

1. **Build the Docker Image:**

   ```bash
   docker build -t simple-copilot-frontend .
   ```

1. **Run the Docker Container:**

   ```bash
   docker run -p 80:80 -d simple-copilot-frontend
   ```