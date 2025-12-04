<template>
  <div class="bg-white p-6 rounded-lg shadow-md border-2 border-blue-200">
    <h3 class="text-xl font-bold text-blue-600 mb-4">
      {{ isEditing ? '✏️ Editar Herói' : '➕ Novo Herói' }}
    </h3>

    <form @submit.prevent="submitForm" class="space-y-4">
      <!-- Nome -->
      <div>
        <label class="block text-sm font-medium text-gray-700">Nome</label>
        <input
          v-model="form.name"
          type="text"
          placeholder="Ex: Superman"
          class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          required
        />
        <span v-if="errors.name" class="text-red-500 text-sm">{{ errors.name }}</span>
      </div>

      <!-- Identidade Real -->
      <div>
        <label class="block text-sm font-medium text-gray-700">Identidade Real</label>
        <input
          v-model="form.realIdentity"
          type="text"
          placeholder="Ex: Clark Kent"
          class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          required
        />
        <span v-if="errors.realIdentity" class="text-red-500 text-sm">{{ errors.realIdentity }}</span>
      </div>

      <!-- Descrição -->
      <div>
        <label class="block text-sm font-medium text-gray-700">Descrição</label>
        <textarea
          v-model="form.description"
          placeholder="Descreva o herói"
          rows="3"
          class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
        ></textarea>
        <span v-if="errors.description" class="text-red-500 text-sm">{{ errors.description }}</span>
      </div>

      <!-- Superpoderes -->
<div>
  <label class="block text-sm font-medium text-gray-700 mb-2">
    ✨ Superpoderes
    <span class="text-gray-500 text-xs">(selecione um ou mais)</span>
  </label>
  
  <!-- Loading -->
  <div v-if="loadingPowers" class="text-gray-600 text-sm">
    Carregando superpoderes...
  </div>

  <!-- Lista de superpoderes -->
  <div v-else class="bg-gray-50 p-3 rounded-lg border border-gray-300 max-h-48 overflow-y-auto">
    <div v-if="availablePowers.length === 0" class="text-gray-500 text-sm">
      Nenhum superpoder disponível. Crie um na aba "Poderes".
    </div>

    <div v-for="power in availablePowers" :key="power.id" class="flex items-center mb-2">
      <input
        :id="`power-${power.id}`"
        :checked="isSelectedPower(power.id)"
        type="checkbox"
        class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
        @change="togglePower(power.id)"
      />
      <label :for="`power-${power.id}`" class="ml-2 text-sm cursor-pointer">
        <span class="font-medium">{{ power.name }}</span>
        <span v-if="power.description" class="text-gray-500 text-xs block">
          {{ power.description }}
        </span>
      </label>
    </div>
  </div>

  <!-- Superpoderes selecionados -->
  <div v-if="form.selectedPowers.length > 0" class="mt-3 flex flex-wrap gap-2">
    <div
      v-for="powerId in form.selectedPowers"
      :key="powerId"
      class="bg-blue-100 text-blue-800 px-3 py-1 rounded-full text-sm flex items-center gap-2"
    >
      ✨ {{ getPowerName(powerId) }}
      <button
        type="button"
        @click="removePowerFromForm(powerId)"
        class="text-blue-600 hover:text-blue-800 font-bold"
      >
        ✕
      </button>
    </div>
  </div>
</div>
      <!-- Botões -->
      <div class="flex gap-3">
        <button
          type="submit"
          :disabled="isLoading"
          class="flex-1 bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 transition disabled:bg-gray-400"
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
import { ref, computed, defineProps, defineEmits, watch, onMounted } from 'vue'
import { heroService, superPowerService, heroSuperPowerService } from '../services/api'

const isEditing = computed(() => {
  return props.editingHero?.id ? true : false
})

onMounted(() => {
  loadAvailablePowers()
})

const loadAvailablePowers = async () => {
  loadingPowers.value = true
  try {
    const response = await superPowerService.getAll()
    availablePowers.value = Array.isArray(response.data) ? response.data : (Array.isArray(response) ? response : [])
  } catch (error) {
    console.error('Erro ao carregar superpoderes:', error)
  } finally {
    loadingPowers.value = false
  }
}

const isSelectedPower = (powerId) => {
  return form.value.selectedPowers.includes(powerId)
}

const togglePower = (powerId) => {
  const index = form.value.selectedPowers.indexOf(powerId)
  if (index > -1) {
    form.value.selectedPowers.splice(index, 1)
  } else {
    form.value.selectedPowers.push(powerId)
  }
}

const removePowerFromForm = (powerId) => {
  const index = form.value.selectedPowers.indexOf(powerId)
  if (index > -1) {
    form.value.selectedPowers.splice(index, 1)
  }
}

const getPowerName = (powerId) => {
  const power = availablePowers.value.find(p => p.id === powerId)
  return power ? power.name : 'Poder desconhecido'
}

const props = defineProps({
  editingHero: Object
})

const emit = defineEmits(['heroAdded', 'heroUpdated'])

const form = ref({
  name: '',
  realIdentity: '',
  description: '',
  selectedPowers: []
})

const availablePowers = ref([])
const loadingPowers = ref(false)

const errors = ref({})
const isLoading = ref(false)
const message = ref({ text: '', type: '' })

watch(() => props.editingHero, (newHero) => {
  if (newHero) {
    form.value = {
      name: newHero.name,
      realIdentity: newHero.realIdentity,
      description: newHero.description || '',
      selectedPowers: newHero.heroSuperPowers?.map(hp => hp.superPowerId) || [] 
    }
  }
}, { deep: true })

const validateForm = () => {
  errors.value = {}
  if (!form.value.name?.trim()) errors.value.name = 'Nome obrigatório'
  if (!form.value.realIdentity?.trim()) errors.value.realIdentity = 'Identidade real obrigatória'
  return Object.keys(errors.value).length === 0
}

const submitForm = async () => {
  if (!validateForm()) return

  isLoading.value = true
  message.value = { text: '', type: '' }

  try {
    if (props.editingHero?.id) {
      await heroService.update(props.editingHero.id, form.value)
      message.value = { text: '✅ Herói atualizado com sucesso!', type: 'success' }
      if (heroId && form.value.selectedPowers.length > 0) {
	  try {
	    for (const powerId of form.value.selectedPowers) {
	      await heroSuperPowerService.associatePower(heroId, powerId)
	    }
	  } catch (error) {
	    console.error('Erro ao associar superpoderes:', error)
	  }
	}
	isEditing.value ? emit('heroUpdated') : emit('heroAdded')
    } else {
      await heroService.create(form.value)
      message.value = { text: '✅ Herói criado com sucesso!', type: 'success' }
      emit('heroAdded')
    }
    resetForm()
  } catch (error) {
    message.value = { text: `❌ Erro: ${error.message}`, type: 'error' }
  } finally {
    isLoading.value = false
  }
}

const resetForm = () => {
  form.value = { name: '', realIdentity: '', description: '', selectedPowers: [] }
  errors.value = {}
}
</script>

<style scoped>
input, textarea {
  transition: all 0.3s ease;
}

input:focus, textarea:focus {
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}
</style>