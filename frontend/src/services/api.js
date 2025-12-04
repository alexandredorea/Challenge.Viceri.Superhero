import axios from 'axios'

const API_URL = import.meta.env.VITE_API_URL || 'https://localhost:7220/api'

const apiClient = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

// ============ HEROES ============

export const heroService = {
  // Listar todos os heróis
  getAll: async () => {
    try {
      const response = await apiClient.get('/heroes')
      return response.data
    } catch (error) {
      console.error('Erro ao listar heróis:', error)
      throw error
    }
  },

  // Obter herói por ID
  getById: async (id) => {
    try {
      const response = await apiClient.get(`/heroes/${id}`)
      return response.data
    } catch (error) {
      console.error(`Erro ao obter herói ${id}:`, error)
      throw error
    }
  },

  // Criar novo herói
  create: async (heroData) => {
    try {
      const response = await apiClient.post('/heroes', heroData)
      return response.data
    } catch (error) {
      console.error('Erro ao criar herói:', error)
      throw error
    }
  },

  // Atualizar herói
  update: async (id, heroData) => {
    try {
      const response = await apiClient.put(`/heroes/${id}`, heroData)
      return response.data
    } catch (error) {
      console.error(`Erro ao atualizar herói ${id}:`, error)
      throw error
    }
  },

  // Deletar herói
  delete: async (id) => {
    try {
      const response = await apiClient.delete(`/heroes/${id}`)
      return response.data
    } catch (error) {
      console.error(`Erro ao deletar herói ${id}:`, error)
      throw error
    }
  }
}

// ============ SUPER POWERS ============

export const superPowerService = {
  // Listar todos os superpoderes
  getAll: async () => {
    try {
      const response = await apiClient.get('/superpowers')
      return response.data
    } catch (error) {
      console.error('Erro ao listar superpoderes:', error)
      throw error
    }
  },

  // Obter superpoder por ID
  getById: async (id) => {
    try {
      const response = await apiClient.get(`/superpowers/${id}`)
      return response.data
    } catch (error) {
      console.error(`Erro ao obter superpoder ${id}:`, error)
      throw error
    }
  },

  // Criar novo superpoder
  create: async (powerData) => {
    try {
      const response = await apiClient.post('/superpowers', powerData)
      return response.data
    } catch (error) {
      console.error('Erro ao criar superpoder:', error)
      throw error
    }
  },

  // Atualizar superpoder
  update: async (id, powerData) => {
    try {
      const response = await apiClient.put(`/superpowers/${id}`, powerData)
      return response.data
    } catch (error) {
      console.error(`Erro ao atualizar superpoder ${id}:`, error)
      throw error
    }
  },

  // Deletar superpoder
  delete: async (id) => {
    try {
      const response = await apiClient.delete(`/superpowers/${id}`)
      return response.data
    } catch (error) {
      console.error(`Erro ao deletar superpoder ${id}:`, error)
      throw error
    }
  }
}


export default apiClient