import { useActionState } from 'react';
import { GoogleGenAI } from '@google/genai';

interface FormState {
  worksheet: string | null;
  error: string | null;
}

const initialState: FormState = {
  worksheet: null,
  error: null,
};

async function generateWorksheet(
  _prevState: FormState,
  formData: FormData,
): Promise<FormState> {
  const gradeLevel = formData.get('gradeLevel')?.toString().trim();
  const subject = formData.get('subject')?.toString().trim();
  const topic = formData.get('topic')?.toString().trim();

  if (!gradeLevel || !subject || !topic) {
    return { worksheet: null, error: 'Please fill out all fields.' };
  }

  // Use import.meta.env for Vite (or create a backend proxy to protect this key)
  const apiKey = import.meta.env.VITE_GEMINI_API_KEY;
  if (!apiKey) {
    return {
      worksheet: null,
      error: 'Missing VITE_GEMINI_API_KEY in your environment configuration.',
    };
  }

  try {
    const ai = new GoogleGenAI({ apiKey });
    const prompt = `Create a 10-question ${subject} worksheet for ${gradeLevel} grade students covering ${topic}.`;

    // Official @google/genai call signature
    const interaction = await ai.interactions.create({
      model: 'gemini-3.8-flash',
      input: prompt,
    });

    return {
      worksheet: interaction.output_text ?? 'No worksheet generated.',
      error: null,
    };
  } catch (err) {
    const message =
      err instanceof Error ? err.message : 'An unexpected error occurred.';
    return {
      worksheet: null,
      error: message,
    };
  }
}

export default function App() {
  const [state, formAction, isPending] = useActionState(
    generateWorksheet,
    initialState,
  );

  return (
    <div className="p-6 max-w-2xl mx-auto font-sans">
      <h2 className="text-2xl font-bold mb-6">AI Worksheet Generator</h2>

      <form action={formAction} className="flex flex-col gap-4">
        <div className="flex gap-4">
          <input
            type="text"
            name="gradeLevel"
            defaultValue="10th"
            placeholder="Grade Level"
            required
            className="border p-2 rounded flex-1"
          />
          <input
            type="text"
            name="subject"
            defaultValue="Computer Science"
            placeholder="Subject"
            required
            className="border p-2 rounded flex-1"
          />
        </div>

        <input
          type="text"
          name="topic"
          defaultValue="Sorting Algorithms"
          placeholder="Specific Topic"
          required
          className="border p-2 rounded w-full"
        />

        <button
          type="submit"
          disabled={isPending}
          className="bg-blue-600 text-white px-4 py-2 rounded font-medium disabled:opacity-50 transition-opacity">
          {isPending ? 'Generating Worksheet...' : 'Generate Worksheet'}
        </button>
      </form>

      {state.error && (
        <div className="mt-6 p-4 bg-red-50 text-red-700 border border-red-200 rounded-lg text-sm">
          {state.error}
        </div>
      )}

      {state.worksheet && (
        <div className="mt-8 p-6 bg-gray-50 border rounded-lg whitespace-pre-wrap text-left shadow-sm leading-relaxed">
          {state.worksheet}
        </div>
      )}
    </div>
  );
}
