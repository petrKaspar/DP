import re
import numpy as np



"""
def rgb2text222(r, g, b):
    for a in lines:
        print(a)
    return 5

print('/////////////')
print(lines[4])
print(lines[4][1:5])
print('/////////////')
for line in lines:
    print(line,'--------')
    print(line[1], '******')
    print(line[0],'///////')
"""

def rgb2text(r, g, b):
    fname = 'colors.txt'
    lines = [re.split('[=,-]+', lines.strip('\n')) for lines in open(fname)]
    for ele in lines:
        ele.append(0)  # pocitadlo nalezeni
    print(lines)

    for a in lines:
        print(a)
        if (int(a[1]) + 0 <= r <= int(a[2]) + 0) and (int(a[3]) + 0 <= g <= int(a[4]) + 0) and (
                    int(a[5]) + 0 <= b <= int(a[6]) + 0):
            # a[0].append('aaaaaaaaaaa')
            a[7] += 1
            print(r, g, b, " = ", a[0])
            return a[0]
    return ''

def getTextColorFromRGB(r, g, b):

    fname = 'colors.txt'
    lines = [re.split('[=,-]+', lines.strip('\n')) for lines in open(fname)]
    for ele in lines:
        ele.append(0)  # pocitadlo nalezeni
    print(lines)

    for a in lines:
        print(a)
        if (int(a[1])+0  <= r <= int(a[2])+0) and (int(a[3])+0  <= g <= int(a[4])+0) and (int(a[5])+0  <= b <= int(a[6])+0):
            #a[0].append('aaaaaaaaaaa')
            a[7] += 1
            print(r, g, b, " = ",a[0])
            #return a[0]

    maxi = ['', '', '', '', '', '', '', 0]
    for c in lines:
        if c[7] > maxi[7]: maxi = c
    print(maxi)
    return maxi[0]

r = getTextColorFromRGB(20, 27, 165)

print(r)

t = rgb2text(20, 27, 165)
print(t)

