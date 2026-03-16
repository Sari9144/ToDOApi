
import axios from 'axios';

// 1. הגדרת כתובת ה-API כברירת מחדל (Config Defaults)
const apiUrl = "http://localhost:5045"; 
axios.defaults.baseURL = apiUrl;

// 2. הוספת Interceptor לתפיסת שגיאות ורישום ללוג
axios.interceptors.response.use(
    response => response,
    error => {
        // רישום השגיאה ללוג כפי שנדרש באפיון
        console.error("API Error context:", error.response ? error.response.data : error.message);
        
        // אתגר: תפיסת שגיאה 401 (בעתיד עבור JWT)
        if (error.response && error.response.status === 401) {
            console.warn("Unauthorized! Redirecting to login...");
            // כאן תוסיפי הפניה לדף לוגין בעתיד
        }
        
        return Promise.reject(error);
    }
);

const todoService = {
  // שליפת כל המשימות
  getTasks: async () => {
    const result = await axios.get('/items');
    return result.data;
  },

  // הוספת משימה חדשה
  addTask: async (name) => {
    // שליחת אובייקט תואם למודל Item ב-C#
    const result = await axios.post('/items', { 
        name: name, 
        isComplete: false 
    });
    return result.data;
  },

  // עדכון משימה (חשוב לשלוח אובייקט עם השדות שהשרת מצפה להם)
  setCompleted: async (id, isComplete, name) => {
    const result = await axios.put(`/items/${id}`, { 
        id: id,
        name: name, // מומלץ להעביר את השם כדי שלא יימחק בעדכון
        isComplete: isComplete 
    });
    return result.data;
  },

  // מחיקת משימה
  deleteTask: async (id) => {
    const result = await axios.delete(`/items/${id}`);
    return result.data;
  }
};

export default todoService;