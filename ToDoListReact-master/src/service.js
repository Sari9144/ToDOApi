import axios from 'axios';

// 1. הגדרת baseURL ישירות מה-ENV
// הערך הוא: https://todoapiservice-x9tf.onrender.com/api
axios.defaults.baseURL = process.env.REACT_APP_API_URL;

const todoService = {
  // שליפת כל המשימות
  getTasks: async () => {
    // פונה ל: baseURL + /item => .../api/item
    const result = await axios.get('/item'); 
    return result.data;
  },

  // הוספת משימה חדשה
  addTask: async (name) => {
    const result = await axios.post('/item', { 
        name: name, 
        isComplete: false 
    });
    return result.data;
  },

  // עדכון משימה
  setCompleted: async (id, isComplete, name) => {
    // פונה ל: .../api/item/{id}
    const result = await axios.put(`/item/${id}`, { 
        id: id,
        name: name,
        isComplete: isComplete 
    });
    return result.data;
  },

  // מחיקת משימה
  deleteTask: async (id) => {
    const result = await axios.delete(`/item/${id}`);
    return result.data;
  }
};

export default todoService;