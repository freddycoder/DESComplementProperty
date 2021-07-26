# DESComplementProperty

My attempt to demontrate the complement property of the DES algorithm.

This is my answer to the exercice 3.10 of the Cryptography Engineering book, by Niels Ferguson, Bruce Scheier and Tadayoshi Kohno.

There is an error somewhere. The complement of the cyphertext created with the key and the plain text are suppose to be the same as the cyphertext create with the complement of the key and the complement of the plaintext.

Run the unittest and you'll see that the unittest that test this property failed... I push this code on github for reference in the future.
