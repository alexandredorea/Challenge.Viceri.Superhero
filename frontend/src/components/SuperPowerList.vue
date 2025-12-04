<template>
  <div class="space-y-4">
    <h3 class="text-xl font-bold text-purple-600">✨ Lista de Superpoderes</h3>

    <!-- Loading -->
    <div v-if="isLoading" class="text-center py-8">
      <div class="inline-block animate-spin text-3xl">⚙️</div>
      <p class="mt-2 text-gray-600">Carregando superpoderes...</p>
    </div>

    <!-- Lista vazia -->
    <div v-else-if="powers.length === 0" class="text-center py-8 bg-gray-100 rounded-lg">
      <p class="text-gray-600">Nenhum superpoder cadastrado ainda. Crie um novo! 🚀</p>
    </div>

    <!-- Lista de superpoderes -->
    <div v-else class="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      <div
        v-for="power in powers"
        :key="power.id"
        class="bg-gradient-to-br from-purple-50 to-pink-50 p-4 rounded-lg border-2 border-purple-200 hover:shadow-lg transition"
      >
        <div class="flex justify-between items-start mb-3">
          <h4 class="text-lg font-bold text-purple-600">{{ power.name }}</h4>
          <span class="text-2xl">✨</span>
        </div>

        <p v-if="power.description" class="text-sm text-gray-700 mb-3">
          {{ power.description }}
        </p>

        <div class="flex gap-2">
          <button
            @click="$emit('edit', power)"
            class="flex-1 bg-yellow-500 text-white py-2 rounded hover:bg-yellow-600 transition text-sm"
          >
            ✏️ Editar
          </button>
          <button
            @click="deletePower(power.id)"
            :disabled="deletingId === power.id"
            class="flex-1 bg-red-500 text-white py-2 rounded hover:bg-red-600 transition text-sm disabled:bg-gray-400"
          >
            {{ deletingId === power.id ? '⏳' : '🗑️' }} Deletar
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, defineProps, defineEmits } from 'vue'
import { superPowerService } from '../services/api'

defineProps({
  powers: Array
})

const emit = defineEmits(['edit', 'reload'])

const deletingId = ref(null)

const deletePower = async (id) => {
  if (!confirm('Tem certeza que deseja deletar este superpoder?')) return

  deletingId.value = id
  try {
    await superPowerService.delete(id)
    alert('✅ Superpoder deletado com sucesso!')
    emit('reload')
  } catch (error) {
    alert(`❌ Erro ao deletar: ${error.message}`)
  } finally {
    deletingId.value = null
  }
}
</script>

<style scoped>
.animate-spin {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}
</style>