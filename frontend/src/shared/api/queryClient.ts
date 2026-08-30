import { MutationCache, QueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { getApiMessage } from './errors';

export const queryClient = new QueryClient({
  mutationCache: new MutationCache({
    onError: (error, _variables, _context, mutation) => {
      if (mutation.meta?.silence) {
        return;
      }

      toast.error(getApiMessage(error));
    },
  }),
});
