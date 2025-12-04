<template>
  <div class="space-y-6">
    <div class="grid md:grid-cols-3 gap-6">
      <div class="md:col-span-1">
        <SuperPowerForm
          :editingPower="editingPower"
          @powerAdded="loadPowers"
          @powerUpdated="cancelEdit"
        />
      </div>
      <div class="md:col-span-2">
        <SuperPowerList
          :powers="powers"
          @edit="editPower"
          @reload="loadPowers"
        />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import SuperPowerForm from '../components/SuperPowerForm.vue'
import SuperPowerList from '../components/SuperPowerList.vue'
import { superPowerService } from '../services/api'

const powers = ref([])
const editingPower = ref(null)

onMounted(() => {
  loadPowers()
})

const loadPowers = async () => {
  try {
    const response = await superPowerService.getAll()
    powers.value = Array.isArray(response.data) ? response.data : (Array.isArray(response) ? response : [])
  } catch (error) {
    console.error('Erro ao carregar superpoderes:', error)
  }
}

const editPower = (power) => {
  editingPower.value = power
}

const cancelEdit = () => {
  editingPower.value = null
  loadPowers()
}
</script>