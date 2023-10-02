# Chrome passwords dumper

The code will dump every password that chrome saved, into a json file.

**Steps**
+ Locate **local state** file
+ Get the encryption key from the file
+ Locate the **local data** file
+ Get every user data in the file
+ Decrypt all the passwors using **AES**

> Note tis works only in windows
