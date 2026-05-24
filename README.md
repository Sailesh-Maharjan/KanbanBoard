<h1 align="center" id="title">Real-Time Collaborative Kanban Board (.NET 8 + SignalR + PostgreSQL)</h1>




<p id="description">
A real-time collaborative Kanban board built with ASP.NET Core 8 and SignalR WebSockets, allowing multiple users to simultaneously create, edit, drag, reorder, and manage cards with changes instantly synchronized across all connected clients. Boards are dynamically loaded through a dropdown based selection, while PostgreSQL on Render provides persistent cloud storage. The application follows a clean architecture with well organized separation of concerns using Models, DTOs, Services, Hubs, and Controllers
</p>

![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![SignalR](https://img.shields.io/badge/WebSocket-SignalR-blueviolet)
![PostgreSQL](https://img.shields.io/badge/Database-PostgreSQL-336791)
![EF Core](https://img.shields.io/badge/ORM-EF%20Core-orange)
![Render](https://img.shields.io/badge/Deployment-Render-purple)
![Swagger](https://img.shields.io/badge/Docs-Swagger-green)

---

<h2>🚀 Demo</h2>

**Base URL:**  
[`https://kanbanboard-cy9b.onrender.com`](https://kanbanboard-cy9b.onrender.com/)

**Swagger Documentation:**  
[`https://kanbanboard-cy9b.onrender.com/swagger/index.html`](https://kanbanboard-cy9b.onrender.com/swagger/index.html)

**SignalR Hub Endpoint:**  
[`https://kanbanboard-cy9b.onrender.com/kanbanHub`](https://kanbanboard-cy9b.onrender.com/kanbanHub)

> Open the app in at least two browser windows, join the same board, and watch changes sync in real time.

---

<h2>🧐 Features</h2>

Here're some of the project's best features:

*   Real-time collaboration using ASP.NET Core SignalR (WebSockets), where all card actions (create, update, move, delete) are instantly synced across users via board based SignalR groups (board-{id}), with no page refresh required.
*   Drag and drop card management using JavaScript events (dragstart, dragover, drop) with proper ordering logic to maintain correct card sequence.
*  Dynamic board selection via dropdown that auto loads available boards from the REST API.
*   The application is deployed on Render for Production.

### System Architecture
   ![SystemArchitecture](Screenshots/block-diagram.png) 
   
<h2>💻 Built with</h2>

Technologies used in the project:

*   .NET 8 Web API
*   ASP.NET Core SignalR
*   Entity Framework Core
*   PostgreSQL (Render Managed)
*   HTML
*   CSS
*   Vanilla JavaScript
*   Render Platform (Deployment)
*   Docker

  <h2>Project Screenshots:</h2>
