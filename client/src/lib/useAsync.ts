import { useCallback, useEffect, useRef, useState } from 'react'
import { errorMessage } from './api'

export type AsyncState<T> = {
  data: T | undefined
  error: string | undefined
  loading: boolean
  reload: () => void
  setData: (value: T) => void
}

/**
 * Runs `factory` whenever `deps` change and keeps the latest result.
 * `deps` are the values the request depends on; the factory itself is read from a ref so
 * callers do not need to memoise it.
 */
export function useAsync<T>(factory: () => Promise<T>, deps: unknown[], enabled = true): AsyncState<T> {
  const [data, setData] = useState<T | undefined>(undefined)
  const [error, setError] = useState<string | undefined>(undefined)
  const [loading, setLoading] = useState(enabled)
  const [nonce, setNonce] = useState(0)
  const latest = useRef(factory)
  latest.current = factory

  useEffect(() => {
    if (!enabled) { setLoading(false); return }
    let active = true
    setLoading(true)
    setError(undefined)
    latest.current()
      .then(value => { if (active) { setData(value); setLoading(false) } })
      .catch(reason => { if (active) { setError(errorMessage(reason)); setLoading(false) } })
    return () => { active = false }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [...deps, enabled, nonce])

  const reload = useCallback(() => setNonce(value => value + 1), [])
  return { data, error, loading, reload, setData }
}
