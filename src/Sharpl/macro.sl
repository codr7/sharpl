(macro trace(in*)
  '(do 
     ,(say "Enter")
     ,in*
     ,(say "Exit")))