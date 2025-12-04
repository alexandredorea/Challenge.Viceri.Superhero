<template>
  <nav class="bg-gradient-to-r from-blue-600 to-purple-600 text-white shadow-lg">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex justify-between items-center h-16">
        <div class="flex items-center gap-3">
          <span class="text-2xl font-bold">⚡ Superhero</span>
        </div>
        <div class="flex gap-6">
          <button
            @click="goTo('heroes')"
            :class="['px-4 py-2 rounded transition', activeTab === 'heroes' ? 'bg-white text-blue-600 font-bold' : 'hover:bg-blue-500']"
          >
            🦸 Heróis
          </button>
          <button
            @click="goTo('powers')"
            :class="['px-4 py-2 rounded transition', activeTab === 'powers' ? 'bg-white text-blue-600 font-bold' : 'hover:bg-blue-500']"
          >
            ✨ Poderes
          </button>
        </div>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { defineProps, defineEmits } from 'vue'

defineProps({
  activeTab: String
})

const emit = defineEmits(['navigate'])

const goTo = (tab) => {
  emit('navigate', tab)
}
</script>

<style scoped>
button {
  transition: all 0.3s ease;
}
</style>