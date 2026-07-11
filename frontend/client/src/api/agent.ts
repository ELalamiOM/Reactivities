import axios from 'axios';
import { toast } from 'react-toastify';
import { router } from '../app/router/Routes';

const sleep = (delay: number) => {
  return new Promise((resolve) => {
    setTimeout(resolve, delay);
  });
};

// Create axios instance with cookie support
const agent = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'https://localhost:5001',
  withCredentials: true, // Enable cookies
});

// Request interceptor
agent.interceptors.request.use((config) => {
  // Add any auth headers if needed
  return config;
});

// Response interceptor for handling errors
agent.interceptors.response.use(
  async (response) => {
    await sleep(1000);
    return response;
  },
  (error) => {
    const { data, status } = error.response || {};

    switch (status) {
      case 400:
        if (data.errors) {
          const modalStateErrors = [];
          for (const key in data.errors) {
            if (data.errors[key]) {
              modalStateErrors.push(data.errors[key]);
            }
          }
          throw modalStateErrors.flat();
        }
        toast.error('Bad request');
        break;
      case 401:
        toast.error('Unauthorized');
        break;
      case 403:
        toast.error('Forbidden');
        break;
      case 404:
        toast.error('Not found');
        break;
      case 500:
        router.navigate('/server-error');
        break;
      default:
        break;
    }

    return Promise.reject(error);
  }
);

/**
 * Captures login submission and handles cookie generation
 * @param email - User email
 * @param password - User password
 * @returns Login response with user data
 */
export const loginWithCookies = async (
  email: string,
  password: string
) => {
  try {
    console.log('[Agent] Initiating login capture for:', email);
    console.log('[Agent] Base URL:', agent.defaults.baseURL);

    // Send login request - the route expects JSON body
    const payload = { email, password };
    console.log('[Agent] Sending payload:', payload);

    const response = await agent.post('/api/login?useCookies=true', payload);

    console.log('[Agent] Login successful');
    console.log('[Agent] Response status:', response.status);
    console.log('[Agent] Response data:', response.data);
    console.log('[Agent] Response headers:', response.headers);

    // Capture and log cookies
    const cookies = document.cookie;
    console.log('[Agent] Current cookies:', cookies);

    if (response.status === 200) {
      toast.success('Login successful - session established');
    }

    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      console.error('[Agent] Full error object:', error);
      console.error('[Agent] Error message:', error.message);
      
      if (error.response) {
        console.error('[Agent] Error response data:', error.response.data);
        console.error('[Agent] Error response status:', error.response.status);
        console.error('[Agent] Error response headers:', error.response.headers);
        toast.error(`Login failed: ${error.response.status} - ${JSON.stringify(error.response.data)}`);
      } else if (error.request) {
        console.error('[Agent] No response received - Error request:', error.request);
        toast.error('No response from server');
      } else {
        console.error('[Agent] Error message:', error.message);
        toast.error(error.message);
      }
    } else {
      console.error('[Agent] Full error object:', error);
      const errorMessage = error instanceof Error ? error.message : String(error);
      console.error('[Agent] Error message:', errorMessage);
      toast.error(errorMessage);
    }
    
    throw error;
  }
};

export const registerUser = async (
  email: string,
  displayName: string,
  password: string
) => {
  const payload = { email, displayName, password };
  const response = await agent.post('/api/account/register', payload);
  return response.data;
};

export const requestPasswordReset = async (email: string) => {
  const payload = { email };
  const response = await agent.post('/api/forgotPassword', payload);
  return response.data;
};

/**
 * Validates if current session is active
 * @returns Boolean indicating if session is valid
 */
export const validateSession = async (): Promise<boolean> => {
  try {
    const response = await agent.get('/api/account/user-info');
    return response.status === 200;
  } catch (error) {
    console.log('[Agent] Session validation failed:', error);
    return false;
  }
};

/**
 * Logs out user and clears cookies
 */
export const logout = async () => {
  try {
    console.log('[Agent] Logging out - clearing cookies');
    await agent.post('/api/account/logout');
    
    // Clear cookies by setting empty values
    document.cookie.split(';').forEach((c) => {
      document.cookie = c
        .replace(/^ +/, '')
        .replace(/=.*/, `=;expires=${new Date().toUTCString()};path=/`);
    });

    toast.success('Logged out successfully');
  } catch (error) {
    console.error('[Agent] Logout failed:', error);
    toast.error('Logout failed');
  }
};

/**
 * Gets current session information
 */
export const getSessionInfo = async () => {
  try {
    const response = await agent.get('/api/account/user-info');
    return response.data;
  } catch (error) {
    console.error('[Agent] Failed to get session info:', error);
    return null;
  }
};

/**
 * Captures cookie information for debugging
 */
export const captureCookieInfo = () => {
  const cookies = document.cookie;
  const cookieArray = cookies.split(';').map((c) => c.trim());

  console.log('[Agent] Cookie Info:');
  console.log('[Agent] Total cookies:', cookieArray.length);
  
  cookieArray.forEach((cookie, index) => {
    const [name] = cookie.split('=');
    console.log(`[Agent] Cookie ${index + 1}: ${name}`);
  });

  return {
    totalCookies: cookieArray.length,
    cookieNames: cookieArray.map((c) => c.split('=')[0]),
    rawCookies: cookies,
  };
};

export default agent;
