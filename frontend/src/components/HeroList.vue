<template>
  <div class="space-y-4">
    <h3 class="text-xl font-bold text-blue-600">🦸 Lista de Heróis</h3>

    <!-- Loading -->
    <div v-if="isLoading" class="text-center py-8">
      <div class="inline-block animate-spin text-3xl">⚙️</div>
      <p class="mt-2 text-gray-600">Carregando heróis...</p>
    </div>

    <!-- Lista vazia -->
    <div v-else-if="heroes.length === 0" class="text-center py-8 bg-gray-100 rounded-lg">
      <p class="text-gray-600">Nenhum herói cadastrado ainda. Crie um novo! 🚀</p>
    </div>

    <!-- Lista de heróis -->
    <div v-else class="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      <div
        v-for="hero in heroes"
        :key="hero.id"
        class="bg-gradient-to-br from-blue-50 to-purple-50 p-4 rounded-lg border-2 border-blue-200 hover:shadow-lg transition"
      >
        <div class="flex justify-between items-start mb-3">
          <div>
            <h4 class="text-lg font-bold text-blue-600">{{ hero.name }}</h4>
            <p class="text-sm text-gray-600">{{ hero.realIdentity }}</p>
          </div>
          <span class="text-2xl">🦸</span>
        </div>

        <p v-if="hero.description" class="text-sm text-gray-700 mb-3">
          {{ hero.description }}
        </p>

        <div class="flex gap-2">
          <button
            @click="$emit('edit', hero)"
            class="flex-1 bg-yellow-500 text-white py-2 rounded hover:bg-yellow-600 transition text-sm"
          >
            ✏️ Editar
          </button>
          <button
            @click="deleteHero(hero.id)"
            :disabled="deletingId === hero.id"
            class="flex-1 bg-red-500 text-white py-2 rounded hover:bg-red-600 transition text-sm disabled:bg-gray-400"
          >
            {{ deletingId === hero.id ? '⏳' : '🗑️' }} Deletar
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, defineProps, defineEmits, onMounted } from 'vue'
import { heroService } from '../services/api'

defineProps({
  heroes: Array
})

const emit = defineEmits(['edit', 'reload'])

const isLoading = ref(false)
const deletingId = ref(null)

const deleteHero = async (id) => {
  if (!confirm('Tem certeza que deseja deletar este herói?')) return

  deletingId.value = id
  try {
    await heroService.delete(id)
    alert('✅ Herói deletado com sucesso!')
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