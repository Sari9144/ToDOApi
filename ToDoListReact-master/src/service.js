import axios from 'axios';

// 1. הגדרת baseURL ישירות מה-ENV
// הערך הוא: https://todoapiservice-x9tf.onrender.com/api
axios.defaults.baseURL = process.env.REACT_APP_API_URL;

const todoService = {
  // שליפת כל המשימות

  // 1. שליפת כל המשימות - לשנות ל-items
  getTasks: async () => {
    const result = await axios.get('/items'); 
    return result.data;
  },

  // 2. הוספת משימה חדשה - לשנות ל-items
  addTask: async (name) => {
    const result = await axios.post('/items', { 
        name: name, 
        isComplete: false 
    });
    return result.data;
  },

  // 3. עדכון משימה - לשנות ל-items
  setCompleted: async (id, isComplete, name) => {
    const result = await axios.put(`/items/${id}`, { 
        id: id,
        name: name,
        isComplete: isComplete 
    });
    return result.data;
  },

  // 4. מחיקת משימה - לשנות ל-items
  deleteTask: async (id) => {
    const result = await axios.delete(`/items/${id}`);
    return result.data;
  }
}
;

export default todoService;