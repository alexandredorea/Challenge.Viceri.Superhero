<template>
  <div class="space-y-6">
    <div class="grid md:grid-cols-3 gap-6">
      <div class="md:col-span-1">
        <HeroForm
          :editingHero="editingHero"
          @heroAdded="loadHeroes"
          @heroUpdated="cancelEdit"
        />
      </div>
      <div class="md:col-span-2">
        <HeroList
          :heroes="heroes"
          @edit="editHero"
          @reload="loadHeroes"
        />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import HeroForm from '../components/HeroForm.vue'
import HeroList from '../components/HeroList.vue'
import { heroService } from '../services/api'

const heroes = ref([])
const editingHero = ref(null)

onMounted(() => {
  loadHeroes()
})

const loadHeroes = async () => {
  try {
    const response = await heroService.getAll()
    heroes.value = Array.isArray(response.data) ? response.data : (Array.isArray(response) ? response : [])
  } catch (error) {
    console.error('Erro ao carregar heróis:', error)
  }
}

const editHero = (hero) => {
  editingHero.value = hero
}

const cancelEdit = () => {
  editingHero.value = null
  loadHeroes()
}
</script>