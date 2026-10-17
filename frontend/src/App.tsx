import { useState } from 'react';
import { GoogleGenAI } from '@google/genai';

export default function App() {
  const [worksheetContent, setWorksheetContent] = useState('');
  const [isStreaming, setIsStreaming] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleGenerate = async (formData: FormData) => {
    const gradeLevel = formData.get('gradeLevel')?.toString().trim();
    const subject = formData.get('subject')?.toString().trim();
    const topic = formData.get('topic')?.toString().trim();

    if (!gradeLevel || !subject || !topic) {
      setError('Please fill out all fields.');
      return;
    }

    const apiKey = import.meta.env.VITE_GEMINI_API_KEY;
    if (!apiKey) {
      setError('Missing VITE_GEMINI_API_KEY in environment configuration.');
      return;
    }

    setIsStreaming(true);
    setError(null);
    setWorksheetContent('');

    try {
      const ai = new GoogleGenAI({ apiKey });
      const prompt = `Create a 10-question ${subject} worksheet for ${gradeLevel} grade students covering ${topic}.`;

      const stream = await ai.interactions.create({
        model: 'gemini-3.8-flash',
        input: prompt,
        stream: true,
      });

      for await (const event of stream) {
        if (event.event_type === 'step.delta' && event.delta?.type === 'text') {
          const chunk = event.delta.text ?? '';
          setWorksheetContent((prev) => prev + chunk);
        }
      }
    } catch (err: unknown) {
      const message =
        err instanceof Error
          ? err.message
          : 'An error occurred during generation.';
      setError(message);
    } finally {
      setIsStreaming(false);
    }
  };

  return (
    <div className="p-6 max-w-2xl mx-auto font-sans">
      <h2 className="text-2xl font-bold mb-6">AI Worksheet Generator</h2>

      <form action={handleGenerate} className="flex flex-col gap-4">
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
          disabled={isStreaming}
          className="bg-blue-600 text-white px-4 py-2 rounded font-medium disabled:opacity-50 transition-opacity">
          {isStreaming ? 'Streaming Worksheet...' : 'Generate Worksheet'}
        </button>
      </form>

      {error && (
        <div className="mt-6 p-4 bg-red-50 text-red-700 border border-red-200 rounded-lg text-sm">
          {error}
        </div>
      )}

      {worksheetContent && (
        <div className="mt-8 p-6 bg-gray-50 border rounded-lg whitespace-pre-wrap text-left shadow-sm leading-relaxed">
          {worksheetContent}
        </div>
      )}
    </div>
  );
}
