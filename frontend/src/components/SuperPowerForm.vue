<template>
  <div class="bg-white p-6 rounded-lg shadow-md border-2 border-purple-200">
    <h3 class="text-xl font-bold text-purple-600 mb-4">
      {{ isEditing ? '✏️ Editar Superpoder' : '➕ Novo Superpoder' }}
    </h3>

    <form @submit.prevent="submitForm" class="space-y-4">
      <!-- Nome do Poder -->
      <div>
        <label class="block text-sm font-medium text-gray-700">Nome do Poder</label>
        <input
          v-model="form.name"
          type="text"
          placeholder="Ex: Super Força"
          class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
          required
        />
        <span v-if="errors.name" class="text-red-500 text-sm">{{ errors.name }}</span>
      </div>

      <!-- Descrição -->
      <div>
        <label class="block text-sm font-medium text-gray-700">Descrição</label>
        <textarea
          v-model="form.description"
          placeholder="Descreva o superpoder"
          rows="3"
          class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
        ></textarea>
        <span v-if="errors.description" class="text-red-500 text-sm">{{ errors.description }}</span>
      </div>

      <!-- Botões -->
      <div class="flex gap-3">
        <button
          type="submit"
          :disabled="isLoading"
          class="flex-1 bg-purple-600 text-white py-2 rounded-lg hover:bg-purple-700 transition disabled:bg-gray-400"
        >
          {{ isLoading ? '⏳ Salvando...' : (isEditing ? '💾 Atualizar' : '➕ Adicionar') }}
        </button>
        <button
          type="button"
          @click="resetForm"
          class="flex-1 bg-gray-300 text-gray-700 py-2 rounded-lg hover:bg-gray-400 transition"
        >
          ❌ Cancelar
        </button>
      </div>
    </form>

    <!-- Mensagem de sucesso/erro -->
    <div v-if="message.text" :class="['mt-4 p-3 rounded', message.type === 'success' ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700']">
      {{ message.text }}
    </div>
  </div>
</template>

<script setup>
import { ref, computed, defineProps, defineEmits, watch } from 'vue'
import { superPowerService } from '../services/api'

const isEditing = computed(() => {
  return props.editingHero?.id ? true : false
})

const props = defineProps({
  editingPower: Object
})

const emit = defineEmits(['powerAdded', 'powerUpdated'])

const form = ref({
  name: '',
  description: ''
})

const errors = ref({})
const isLoading = ref(false)
const message = ref({ text: '', type: '' })

watch(() => props.editingPower, (newPower) => {
  if (newPower) {
    form.value = { ...newPower }
  }
}, { deep: true })

const validateForm = () => {
  errors.value = {}
  if (!form.value.name?.trim()) errors.value.name = 'Nome obrigatório'
  return Object.keys(errors.value).length === 0
}

const submitForm = async () => {
  if (!validateForm()) return

  isLoading.value = true
  message.value = { text: '', type: '' }

  try {
    if (props.editingPower?.id) {
      await superPowerService.update(props.editingPower.id, form.value)
      message.value = { text: '✅ Superpoder atualizado com sucesso!', type: 'success' }
      emit('powerUpdated')
    } else {
      await superPowerService.create(form.value)
      message.value = { text: '✅ Superpoder criado com sucesso!', type: 'success' }
      emit('powerAdded')
    }
    resetForm()
  } catch (error) {
    message.value = { text: `❌ Erro: ${error.message}`, type: 'error' }
  } finally {
    isLoading.value = false
  }
}

const resetForm = () => {
  form.value = { name: '', description: '' }
  errors.value = {}
}
</script>

<style scoped>
input, textarea {
  transition: all 0.3s ease;
}

input:focus, textarea:focus {
  box-shadow: 0 0 0 3px rgba(168, 85, 247, 0.1);
}
</style>