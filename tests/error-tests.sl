(check Error
  (var err _)

  (try [Any:(^[e] (set err e))]
    (fail _ 'bummer))

  (type-of err))