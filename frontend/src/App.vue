<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 via-white to-purple-50">
    <NavBar :activeTab="currentTab" @navigate="navigate" />

    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Heroes Tab -->
      <div v-if="currentTab === 'heroes'">
        <HeroesView />
      </div>

      <!-- SuperPowers Tab -->
      <div v-if="currentTab === 'powers'">
        <SuperPowersView />
      </div>
    </main>

    <!-- Footer -->
    <footer class="bg-gray-900 text-white py-6 mt-12">
      <div class="max-w-7xl mx-auto px-4 text-center">
        <p>⚡ Superhero Management System v1.0</p>
        <p class="text-gray-400 text-sm mt-2">Desenvolvido com Vue.js 3 + Tailwind CSS</p>
      </div>
    </footer>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import NavBar from './components/NavBar.vue'
import HeroesView from './views/HeroesView.vue'
import SuperPowersView from './views/SuperPowersView.vue'

const currentTab = ref('heroes')

const navigate = (tab) => {
  currentTab.value = tab
}
</script>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen',
    'Ubuntu', 'Cantarell', 'Fira Sans', 'Droid Sans', 'Helvetica Neue',
    sans-serif;
}
</style>