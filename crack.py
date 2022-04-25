import random
import string

encryptKey = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ"


def get_random_string(length):
    # choose from all lowercase letter
    letters = string.ascii_uppercase
    return "".join(random.choice(letters) for i in range(length))


def en1(email: str):
    num = 0
    for c in email:
        num = 43 * num + ord(c)
    return num


def en2(email: str):
    num = 0
    for c in email:
        num = (num << 4) + ord(c)
        num2 = num & 0xF0000000
        if num2 != 0:
            num ^= num2 >> 24
            num ^= num2
    return num


def generate_license(email: str):
    if not email:
        raise Exception("need email")
    email = email.lower()
    num = (en1(email) << 32) + en2(email)
    text = ""
    for i in range(12):
        index = (num >> 64 - (i + 1) * 5) & 0x1F
        text += encryptKey[index]
    return text


if __name__ == "__main__":
    email = input("please input email: ")
    print(generate_license(email))
