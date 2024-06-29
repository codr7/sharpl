(^ make-user ()
  (let [@name ""
        @email ""]
    (^set-name [v]
      (set @name v))
    (^set-email [v]
      (set @email v))
    (^to-string []
      (String "User " @name " " @email))
  (^[target args*]
    ((env target) args*))))

(let [u1 (make-user)
      u2 (make-user)]
  (u1 'set-name "Foo")
  (u1 'set-email "foo@foo.com")
  (u2 'set-name "Bar")
  (u2 'set-email "bar@bar.com")
  (say (u1 'to-string))
  (say (u2 'to-string)))

* implement string type call
** array
* add env macro
** emit GetEnv op with ref to vm.Env
*** push value for id on stack
* add slurp arg support
** claes
* add quoting support
** add Form.Quote()
*** claes
*** only implement for id
**** add Symbol type
** add Quote form
** add Quote reader