<template>
  <div class="dialog-card">
    <div class="dialog-header">
      <div>
        <h2>{{ isEditMode ? 'Edit category' : 'Create category' }}</h2>
        <p>{{ isEditMode ? 'Update the category details.' : 'Create a space to organize related tasks.' }}</p>
      </div>
      <button class="icon-close" @click="$emit('close')" type="button">×</button>
    </div>

    <form class="form" @submit.prevent="submitForm">
      <div class="form-group">
        <label for="category-name">Name <span class="required">*</span></label>
        <input id="category-name" v-model.trim="form.name" type="text" placeholder="Work, Home, Errands" />
      </div>

      <div class="form-group">
        <label for="category-description">Description</label>
        <textarea
          id="category-description"
          v-model.trim="form.description"
          rows="3"
          placeholder="Short context for this category"
        ></textarea>
      </div>

      <p v-if="errorMessage" class="error-text">{{ errorMessage }}</p>

      <div class="footer-actions">
        <button class="secondary-btn" @click="$emit('close')" type="button">Cancel</button>
        <button class="primary-btn" type="submit" :disabled="isSubmitting">
          {{ isSubmitting ? 'Saving...' : isEditMode ? 'Save changes' : 'Create category' }}
        </button>
      </div>
    </form>
  </div>
</template>

<script setup>
import { computed, reactive, ref, watch } from 'vue'
import api from '@/tools/api-request'

const props = defineProps({
  mode: {
    type: String,
    default: 'create'
  },
  category: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['saved', 'close'])

const form = reactive({
  name: '',
  description: ''
})

const errorMessage = ref('')
const isSubmitting = ref(false)

const isEditMode = computed(() => props.mode === 'edit')

watch(
  () => props.category,
  () => {
    form.name = props.category?.name || ''
    form.description = props.category?.description || ''
    errorMessage.value = ''
  },
  { immediate: true }
)

const submitForm = async () => {
  errorMessage.value = ''

  if (!form.name) {
    errorMessage.value = 'Category name is required.'
    return
  }

  try {
    isSubmitting.value = true

    if (isEditMode.value && props.category?.id) {
      await api.put(`/api/categories/${props.category.id}`, {
        name: form.name,
        description: form.description || null
      })
    } else {
      await api.post('/api/categories', {
        name: form.name,
        description: form.description || null
      })
    }

    emit('saved')
  } catch (err) {
    errorMessage.value = err.response?.data?.data?.code === 300108
      ? 'Category name already exists.'
      : 'Failed to save category.'
  } finally {
    isSubmitting.value = false
  }
}
</script>

<style scoped>
.dialog-card {
  background: var(--surface-strong);
  border-radius: 24px;
  padding: 24px;
  box-shadow: var(--shadow-strong);
  border: 1px solid var(--border);
}

.dialog-header {
  display: flex;
  justify-content: space-between;
  gap: 16px;
  margin-bottom: 20px;
}

.dialog-header h2 {
  font-size: 24px;
  font-weight: 800;
}

.dialog-header p {
  margin-top: 8px;
  color: var(--text-secondary);
  font-size: 14px;
}

.icon-close {
  border: none;
  background: var(--surface-muted);
  color: var(--text-primary);
  width: 38px;
  height: 38px;
  border-radius: 12px;
  font-size: 22px;
}

.form {
  display: flex;
  flex-direction: column;
  gap: 18px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-size: 14px;
  font-weight: 700;
}

.form-group input,
.form-group textarea {
  width: 100%;
  border: 1px solid var(--border);
  border-radius: 14px;
  padding: 12px 14px;
  background: var(--surface-muted);
  outline: none;
}

.form-group input:focus,
.form-group textarea:focus {
  border-color: rgba(47, 133, 90, 0.5);
  box-shadow: 0 0 0 4px rgba(47, 133, 90, 0.12);
}

.required {
  color: var(--danger);
}

.footer-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}

.primary-btn,
.secondary-btn {
  border: none;
  border-radius: 14px;
  padding: 12px 16px;
  font-weight: 700;
}

.primary-btn {
  background: linear-gradient(135deg, #48bb78 0%, #2f855a 100%);
  color: white;
}

.secondary-btn {
  background: var(--surface-muted);
  color: var(--text-primary);
}

.error-text {
  color: var(--danger);
  background: var(--danger-soft);
  border: 1px solid #fed7aa;
  border-radius: 14px;
  padding: 12px 14px;
}

@media (max-width: 640px) {
  .dialog-card {
    padding: 20px;
  }

  .footer-actions {
    flex-direction: column-reverse;
  }

  .primary-btn,
  .secondary-btn {
    width: 100%;
  }
}
</style>
