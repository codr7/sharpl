(let [c (net/connect (ARG 0):(parse-int:_ (ARG 1)))]
  (say c)
  (close c))