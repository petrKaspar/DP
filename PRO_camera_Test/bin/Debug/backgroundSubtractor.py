import imutils
import numpy as np
import cv2
import matplotlib.pyplot as plt
from PIL import Image
#from PIL import ImageChop
from PIL import ImageOps
from matplotlib import gridspec
from scipy.misc import imread

import time
import scipy.ndimage as ndimage
import re
from imutils import perspective
from imutils import contours
from scipy.spatial import distance as dist

from decimal import *
#im1 = cv2.imread('images\\background.bmp')
#im2 = cv2.imread('images\\currentImage.bmp')

#im1 = cv2.imread('images\\background.bmp')
#im2 = cv2.imread('images\\currentImage.bmp')

# im1 = cv2.imread('images\\imageOfBackground.bmp')
# im2 = cv2.imread('images\\imageOfPadWithObject.bmp')

im1 = cv2.imread('b.bmp')
im2 = cv2.imread('TestCV191016_14-17-57.bmp')

im1 = cv2.imread('Img__231116_15_00_41.bmp')
im2 = cv2.imread('Img__231116_15_00_29.bmp')



current_frame_gray = cv2.cvtColor(im1, cv2.COLOR_BGR2RGB)
previous_frame_gray = cv2.cvtColor(im2, cv2.COLOR_BGR2RGB)

frame_diff = cv2.absdiff(im1, im2)

#cv2.imshow('frame diff ', frame_diff)
def histogram(im, size):
    h = np.zeros(size)
    for y in range(im.shape[0]):
        for x in range(im.shape[1]):
            h[im[y, x]] += 1
    return h

def histogramHue(im, size):
    print('-------------- histogramHue() --------------')
    h = np.zeros(size+1)
    import math
    rounded_value = 0
    for y in range(im.shape[0]):
        for x in range(im.shape[1]):
            pom=(im[y,x,0].astype(float) + im[y,x,1].astype(float) + im[y,x,2].astype(float))
            # je tu ".astype(float)", protoze jinak "RuntimeWarning: overflow encountered in ubyte_scalars"
            if(pom != 0):
                hueValue, s, v = rgb2hsv(im[y,x,0], im[y,x,1], im[y,x,2])
                # rounded_value = math.ceil(hueValue)
                h[int(round(hueValue))] += 1
    return h

def rgb2hsv(r, g, b):
    r, g, b = r/255.0, g/255.0, b/255.0
    mx = max(r, g, b)
    mn = min(r, g, b)
    df = mx-mn
    if mx == mn:
        h = 0
    elif mx == r:
        h = (60 * ((g - b) / df))
        # h = (60 * ((g-b)/df) + 360) % 360
    elif mx == g:
        h = (60 * (2+(b - r) / df))
        # h = (60 * ((b-r)/df) + 120) % 360
    elif mx == b:
        h = (60 * (4+(r - g) / df))
        # h = (60 * ((r-g)/df) + 240) % 360
    if mx == 0:
        s = 0
    else:
        s = df/mx
    v = mx
    return h, s, v

def black_or_b(im1, im2):
    print('--------------- black_or_b() ---------------')
    diff = cv2.subtract(im2, im1)
    # threshold(diff, diff, 30, 255, CV_THRESH_BINARY);
    #
    # cv2.Dilate(diff, diff, None, 18)  # to get object blobs
    # cv2.Erode(diff, diff, None, 10)

    plt.subplot(2,3,1)
    plt.imshow(im1)
    plt.title('Background')
    plt.axis("off")
    plt.subplot(2,3,2)
    plt.imshow(im2)
    plt.title('Original image')
    plt.axis("off")
    plt.subplot(2,3,3)
    plt.imshow(diff)
    plt.title('Diff')
    plt.axis("off")

    #img555 = cv2.medianBlur(diff, 5)
    img555 = cv2.cvtColor(diff, cv2.COLOR_RGB2GRAY)
    #cv2.imshow("img555",img555)

    plt.subplot(234)
    plt.imshow(img555, cmap='gray')
    plt.title('Diff gray')
    plt.axis("off")

    #
    # h = histogram(diff)
    # plt.plot(h, '-r')
    # plt.figure()
    kernel = np.ones((5, 5), np.uint8)
    #ret,th3 = cv2.threshold(img555,threshold,255,cv2.THRESH_BINARY)
    threshold, th3 = cv2.threshold(img555, 0, 255, cv2.THRESH_BINARY + cv2.THRESH_OTSU)   #automaticky se vybere vhodny prah na zaklade hist, ktery se take zjisti sam
    print ("Auto threshold = {}".format(threshold))
    cv2.imshow('{} = auto threshold'.format(threshold),th3)


    """
    immmm = Image.fromarray(th3)
    immmm.save("images\\testovaci.bmp")
    """

    # ======================================================================
    # label kazde osamocene oblasti prida nejake cislo pro kazdy pixel. Kterych cisel bude nejvice, ponecha se dana oblast v obraze
    label_im, nb_labels = ndimage.label(th3)
    print(nb_labels)
    sizes = ndimage.sum(th3, label_im, range(nb_labels + 1))
    mask_size = sizes != sizes.max()
    remove_pixel = mask_size[label_im]
    label_im[remove_pixel] = 0
    label_im[label_im > 0] = 1
    #
    # plt.figure("llllllaaabbbeeeellllllsssssssll")
    # plt.imshow(label_im, cmap="spectral", interpolation='nearest')
    #
    # cv2.imshow('current_output LABEL', label_im)
    #
    # # Identify discrete regions and assign unique IDs
    # current_output, num_ids = ndimage.label(th3, structure=np.ones((3, 3)))
    # print(current_output.max(axis=1),'mmmmmmmmmmmmmmm')
    # print(num_ids,'current_outputcurrent_outputcurrent_outputcurrent_outputcurrent_outputcurrent_output')
    # # Plot outputs
    # plt.figure("labellllllllll")
    # plt.imshow(th3, cmap="spectral", interpolation='nearest')
    # plt.figure("llllllaaabbbeeeellllllll")
    # plt.imshow(current_output, cmap="spectral", interpolation='nearest')
    # cv2.imshow('current_output LABEL', current_output)


    # ======================================================================


    th3 = cv2.morphologyEx(th3, cv2.MORPH_OPEN, kernel)
    th3 = cv2.dilate(th3,kernel,iterations = 1)

    # #-----------------------
    # des = cv2.bitwise_not(th3)
    # qq, contour, hier = cv2.findContours(des, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    #
    # for cnt in contour:
    #     cv2.drawContours(des, [cnt], 0, 255, -1)
    #
    # th3 = cv2.bitwise_not(des)
    # # -----------------------

    #kernel = cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (13, 13))
    #res = cv2.morphologyEx(th3, cv2.MORPH_OPEN, kernel)

    # #======================
    # # Copy the thresholded image.
    # im_floodfill = th3.copy()
    #
    # # Mask used to flood filling.
    # # Notice the size needs to be 2 pixels than the image.
    # h, w = th3.shape[:2]
    # mask = np.zeros((h + 2, w + 2), np.uint8)
    #
    # # Floodfill from point (0, 0)
    # cv2.floodFill(im_floodfill, mask, (0, 0), 255);
    #
    # # Invert floodfilled image
    # im_floodfill_inv = cv2.bitwise_not(im_floodfill)
    #
    # # Combine the two images to get the foreground.
    # im_out = th3 | im_floodfill_inv
    # # ======================

    """
    th33 = ndi.binary_fill_holes(th3)
    plt.subplot(2, 3, 5)
    plt.imshow(label_im, 'gray')
    plt.title('Binary image')
    plt.axis("off")
    """
    #label_im_Gray = cv2.cvtColor(label_im, cv2.COLOR_)
    nPixelsNonZero = cv2.countNonZero(label_im)
    print('nObjectPixelsNonZero = ',nPixelsNonZero)
    print('nAllPixels = ',label_im.shape[0] * label_im.shape[1])

    for w in range(0, label_im.shape[1]):
        label_im[0,w] = 0
        #label_im[label_im.shape[1]-5, w] = 0


    # velikost strukturniho elementu by nemela byt vetsi nez je 70% puvodniho objektu  (nPixelsNonZero-(nPixelsNonZero / 3))
    kernel_size = int(np.math.sqrt((nPixelsNonZero / 8)))
    #kernel_size = int(np.math.sqrt((nPixelsNonZero / 1.5)))
    print("kernel_size = ",kernel_size)
    kernel = np.ones((kernel_size, kernel_size), np.uint8)
    opening = cv2.morphologyEx(np.uint8(label_im), cv2.MORPH_CLOSE, kernel)

    #kernel = np.ones((5, 5), np.uint8)
    dilation = cv2.dilate(np.uint8(label_im), kernel, iterations=1)
    erosion = cv2.erode(np.uint8(dilation), kernel, iterations=1)

    nPixelsNonZero = cv2.countNonZero(erosion)
    print('nObjectPixelsNonZero = ', nPixelsNonZero)
    print('nAllPixels = ', erosion.shape[0] * erosion.shape[1])

    import math
    nVystupkuu  = math.round = lambda num, n: math.floor((nPixelsNonZero/640) * 10 ** n + 0.5) / 10 ** n

    print("Pocet vystupkuu = ", round(nPixelsNonZero/640))


    plt.subplot(2, 3, 5)
    plt.imshow(erosion, 'gray')
    plt.title('Binary image')
    plt.axis("off")


    fg = cv2.bitwise_or(im2, im2, mask=erosion)
    x, y, width, height = cv2.boundingRect(erosion)
    roi = fg[y:y + height, x:x + width]

    roi_border = addBorder(roi, 30)

    ret, thresh1 = cv2.threshold(cv2.cvtColor(roi_border, cv2.COLOR_RGB2GRAY), 1, 255, cv2.THRESH_BINARY)

    stranaA, stranaB = measurSize(roi_border, thresh1)

    # plt.subplot(236)
    #plt.subplot(2,3,(3,6))
    plt.subplot(236)
    plt.subplots_adjust(bottom=0.015, left=0.025, top=0.988, right=0.98, wspace=0.1, hspace=0.1)
    plt.imshow(roi_border, interpolation='nearest')
    plt.text(0,0,"vystupky = {} (podle poctu pix.)\nkostka {}x{}".format(round(nPixelsNonZero/640),round(stranaA), round(stranaB)),fontsize=11,
             bbox={'facecolor': 'red', 'alpha': 0.5, 'pad': 3}, color='white')
    plt.axis("off")
    plt.savefig('images\\saveFig{}.png'.format(int(time.time())))

    #roi = cv2.cvtColor(roi, cv2.COLOR_BGR2RGB)

    return erosion, roi, roi_border, stranaA, stranaB


    mask_inv = cv2.bitwise_not(th3)

    rows, cols, channels = im2.shape
    roi = im2[0:rows, 0:cols]

    # Now black-out the area of logo in ROI
    img1_bg = cv2.bitwise_and(roi, roi, mask =mask_inv)
    #cv2.imshow('img1_bg',img1_bg)
    #mask33 = cv2.bitwise_not(th33)
    # Take only region of logo from logo image.
    from skimage import img_as_ubyte
    cv_image = img_as_ubyte(label_im)  #img_as_ubyte(th33) ; #po vyplneni der se z nejakeho duvodu musi obrazek prevest zpet na format cv2...
    img2_fg = cv2.bitwise_and(im2, im2, mask=cv_image)
    img2_fg = cv2.cvtColor(img2_fg, cv2.COLOR_BGR2RGB)
    #cv2.imshow('img1_bg', img2_fg)
    #return diff


    im, contours, hierarchy = cv2.findContours(th3, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
    #nalezeni nejvetsi oblasti
    largest_areas = sorted(contours, key=cv2.contourArea)
    cv2.drawContours(th3, contours, -1, (250, 250, 250), 2)
    # cv2.imshow('your_image.jpg', im_out)

    #########cv2.waitKey()
    #########cv2.destroyAllWindows()
    print("len(contours) = ",len(contours))
    if (len(contours)) != 0 :
        cnt = contours[0]
        cnt = largest_areas[-1]
        area = cv2.contourArea(cnt)
        # print(area)

        x, y, width, height = cv2.boundingRect(cnt)
        roi = img2_fg[y:y + height, x:x + width]
        #cv2.imshow('roi', roi)
        #cv2.imwrite("roi.png", roi)

        rect = cv2.minAreaRect(cnt)
        box = cv2.boxPoints(rect)
        box = np.int0(box)
        cv2.drawContours(im2, [box], 0, (0, 0, 255), 2)

        M = cv2.moments(cnt)
        cv2.drawContours(im2, [cnt], 0, (0, 255, 0), 3)
        #cv2.imshow('output', im2)

    # plt.subplot(236)
    # plt.imshow(cv2.cvtColor(roi, cv2.COLOR_BGR2RGB))
    # plt.axis("off")
    # plt.savefig('images\\saveFig.png')

    return roi

def measurSize(original, binImage):
    print('--------------- measurSize() ---------------')
    # find contours in the edge map
    cnts = cv2.findContours(binImage.copy(), cv2.RETR_EXTERNAL,
                            cv2.CHAIN_APPROX_SIMPLE)
    cnts = cnts[0] if imutils.is_cv2() else cnts[1]

    # sort the contours from left-to-right and initialize the
    # 'pixels per metric' calibration variable
    (cnts, _) = contours.sort_contours(cnts)
    pixelsPerMetric = None

    # loop over the contours individually
    for c in cnts:
        # if the contour is not sufficiently large, ignore it
        if cv2.contourArea(c) < 100:
            continue

        # rotuje kolem objektu obdelnik, a hleda nejmensi mozny, do ktereho se cely objekt schova
        # compute the rotated bounding box of the contour
        box = cv2.minAreaRect(c)
        box = cv2.cv.BoxPoints(box) if imutils.is_cv2() else cv2.boxPoints(box)
        box = np.array(box, dtype="int")

        # order the points in the contour such that they appear
        # in top-left, top-right, bottom-right, and bottom-left
        # order, then draw the outline of the rotated bounding
        # box
        box = perspective.order_points(box)
        cv2.drawContours(original, [box.astype("int")], -1, (0, 255, 0), 2)

        # loop over the original points and draw them
        for (x, y) in box:
            cv2.circle(original, (int(x), int(y)), 5, (0, 0, 255), -1)

        # unpack the ordered bounding box, then compute the midpoint
        # between the top-left and top-right coordinates, followed by
        # the midpoint between bottom-left and bottom-right coordinates
        (tl, tr, br, bl) = box
        (tltrX, tltrY) = midpoint(tl, tr)
        (blbrX, blbrY) = midpoint(bl, br)

        # compute the midpoint between the top-left and top-right points,
        # followed by the midpoint between the top-righ and bottom-right
        (tlblX, tlblY) = midpoint(tl, bl)
        (trbrX, trbrY) = midpoint(tr, br)

        # draw the midpoints on the image
        cv2.circle(original, (int(tltrX), int(tltrY)), 5, (255, 0, 0), -1)
        cv2.circle(original, (int(blbrX), int(blbrY)), 5, (255, 0, 0), -1)
        cv2.circle(original, (int(tlblX), int(tlblY)), 5, (255, 0, 0), -1)
        cv2.circle(original, (int(trbrX), int(trbrY)), 5, (255, 0, 0), -1)

        # draw lines between the midpoints
        cv2.line(original, (int(tltrX), int(tltrY)), (int(blbrX), int(blbrY)),
                 (255, 0, 255), 2)
        cv2.line(original, (int(tlblX), int(tlblY)), (int(trbrX), int(trbrY)),
                 (255, 0, 255), 2)

        # compute the Euclidean distance between the midpoints
        dA = dist.euclidean((tltrX, tltrY), (blbrX, blbrY))
        dB = dist.euclidean((tlblX, tlblY), (trbrX, trbrY))

        # if the pixels per metric has not been initialized, then
        # compute it as the ratio of pixels to supplied metric
        # (in this case, inches)
        if pixelsPerMetric is None:
            pixelsPerMetric = dB / 8
        print(pixelsPerMetric,' = pixelsPerMetric')
        pixelsPerMetric = 25.2982
        # compute the size of the object
        dimA = dA / pixelsPerMetric
        dimB = dB / pixelsPerMetric
        # B je delsi
        # A je ta kratsi strana
        # draw the object sizes on the image
        cv2.putText(original, "{:.1f}".format(dimA),
                    (int(tltrX - 15), int(tltrY - 10)), cv2.FONT_HERSHEY_DUPLEX,
                    0.48, (255, 255, 255), 2)
        cv2.putText(original, "{:.1f}".format(dimB),
                    (int(trbrX + 10), int(trbrY)), cv2.FONT_HERSHEY_DUPLEX,
                    0.48, (255, 255, 255), 2)

        # show the output image
        #cv2.imshow("Image", original)

    return dimA, dimB

def midpoint(ptA, ptB):
    return ((ptA[0] + ptB[0]) * 0.5, (ptA[1] + ptB[1]) * 0.5)

def addBorder(old_img, border_size):
    # prida k obrazu cerby ramecek s sirkou dle libosti

    # old_size = old_img.shape[0], old_img.shape[1]
    # new_size = (old_img.shape[0] + border_size, old_img.shape[1] + border_size)
    # new_im = Image.new("RGB", new_size, "white")  ## luckily, this is already black!
    # new_im.paste(Image.fromarray(old_img), (int((new_size[0] - old_size[0]) / 2), int((new_size[1] - old_size[1]) / 2)))
    new_im = np.array(ImageOps.expand(Image.fromarray(old_img), border=border_size, fill='black'))
    return new_im

def foundObjectToGray(foundObject):
    img_gray = cv2.cvtColor(foundObject, cv2.COLOR_RGB2GRAY)
    hist_img_gray, b = np.histogram(img_gray, 255, (1, 255))
    maxXwMY = hist_img_gray.argmax()
    maxY = max(hist_img_gray)
    """
    plt.figure('aaaaaaa',dpi=50)
    plt.subplot(121)
    plt.imshow(img_gray, cmap='gray')
    plt.subplot(122)
    plt.imshow(cv2.cvtColor(foundObject, cv2.COLOR_BGR2RGB))
    """
    aaa = "Max = [{0}, {1}]".format(maxXwMY, maxY)
    print(aaa)
    plt.figure('hist_img_gray')
    #plt.plot(hist_img_gray)
    plt.plot(hist_img_gray, marker='.')
    plt.grid(True)
    plt.axvline(x=maxXwMY, ymin=0, ymax=maxXwMY, linewidth=1, color='r')
    plt.text(maxXwMY, maxY, aaa)
    plt.xlabel('GRAYSCALE VALUE')
    plt.ylabel('NUMBER')
    plt.title('Histogram of grayscale image')

    """ ////////////////////////////////
    udelat hlavni figuru 3x3, misto 2x3
        * stred diff g, hist, thresh + prah
        *
    Porc nefunguje nejvetsi Obj
        zkusit na :
        im1 = cv2.imread('Img__231116_15_22_06.bmp')
        im2 = cv2.imread('Img__231116_15_20_29.bmp')
    //////////////////////////////// """

    plt.savefig('images\\saveFig_Gray.png')
    # print(img_gray.mean())
    # print(hist_img_gray.mean())
    # print(hist_img_gray.max(axis=0))
    # print(np.argwhere(hist_img_gray == hist_img_gray.max()))
    print("hist_img_gray.argmax() = ",hist_img_gray.argmax())
    return maxXwMY


def foundObject_hue(foundObject):
    img_hsv = cv2.cvtColor(foundObject, cv2.COLOR_BGR2HSV)
    # cv2.imshow('images\\aaaaaaaaaaaaaaaa', foundObject)
    #hist_hue_img, b = np.histogram(img_hsv[:, :, 0], 360, (0, 256))
    #hist_hue_img = histogram(img_hsv[:, :, 0], 360)
    #hist_hue_img, s, v = rgb2hsv(foundObject[:,0,0],0,0)
    hist_hue_img = histogramHue(foundObject, 360)

    # cv2.imshow('hsvHrnek', img_hsv)
    maxXwhereMaxY = hist_hue_img.argmax()
    maxY = max(hist_hue_img)
    aaa = "Max = [{0}, {1}]".format(maxXwhereMaxY, int(maxY))
    print("Hue ",aaa)

    imgBack = plt.imread("HueScale.png")

    plt.figure('hist_hue_img', figsize=(6, 5), dpi=100)
    plt.axis('auto')
    #plt.autoscale_view(True, True, True)
    plt.xlim(0,360)
    plt.ylim(0,maxY+200)
    plt.imshow(imgBack, zorder=0, extent=[0, 360, 0, maxY+200],  aspect='auto')
    plt.plot(hist_hue_img, marker='.', zorder=1, color='black', linewidth=2, markeredgewidth=3)
    plt.grid(True)
    plt.text(maxXwhereMaxY, maxY, aaa)
    plt.title('Histogram for hue values')
    plt.xlabel('HUE VALUE')
    plt.ylabel('NUMBER')

    plt.savefig('images\\saveFig_Hue.png')
    # print(img_hsv[:, :, 0].mean())
    # print(hist_hue_img.mean())
    # print(hist_hue_img.max(axis=0))
    # print(np.argwhere(hist_hue_img == hist_hue_img.max()))
    print("hist_hue_img.argmax() = ",hist_hue_img.argmax())
    return maxXwhereMaxY

def getTextColorFromRGB(img5):

    fname = 'colors343.txt'
    lines = [re.split('[=,-]+', lines.strip('\n')) for lines in open(fname)]
    for ele in lines:
        ele.append(0)  # pocitadlo nalezeni
    print(lines)

    for y in range(img5.shape[0]):
        for x in range(img5.shape[1]):
            r = img5[y][x][0]
            g = img5[y][x][1]
            b = img5[y][x][2]
            #if r > 0 or g > 0 or b > 0:
            if r + g + b > 15:
                for a in lines:
                    #print(a)
                    if (int(a[1])+0  <= r <= int(a[2])+0) and (int(a[3])+0  <= g <= int(a[4])+0) and (int(a[5])+0  <= b <= int(a[6])+0):
                        #a[0].append('aaaaaaaaaaa')
                        a[7] += 1
                        break
                        ##############print(r, g, b, " = ",a[0])
                        #return a[0]

    maxi = ['', '', '', '', '', '', '', 0]
    linesSorted = sorted(lines, key=lambda zaznam: zaznam[7])
    for c in linesSorted:
        if (c[7] > 0): print(c[0], ' - ', c[7])
        if c[7] > maxi[7]: maxi = c

    print('*****************\n',maxi,'\n*****************')
    return maxi[0]
def chain_median(chain2, sizeStrucElement):
    #print(sizeStrucElement / 2)
    #print(int(sizeStrucElement / 2))
    chain3=[]
    halfElement = int(sizeStrucElement / 2)
    for i in range(len(chain2)):
        #print(np.median(chain2[i-halfElement : i+halfElement]))
        l = i - halfElement
        r = i + halfElement
        if l < 0: l=0
        if r > len(chain2): r =len(chain2)
        newVal=(np.median(chain2[l : r]))
        chain3.append(int(newVal))

    return chain3

def chain_code(image_bin):
    print('--------------- chain_code() ---------------')
    kernel = np.ones((3, 3), np.uint8)
    erosion = cv2.erode(np.uint8(image_bin), kernel, iterations=1)

    image_contour = (image_bin) - (erosion)
    plt.figure()
    plt.imshow(image_contour, cmap='gray')

    # najde souradnice prvni jednicky binarniho obrazu, na kterou narazi. Nasledne ze smycek vyskoci
    class Found(Exception): pass
    try:
        for y in range(image_contour.shape[0]):
            for x in range(image_contour.shape[1]):
                if image_contour[y][x] == 1:
                    raise Found
    except Found:
        start_point = (y, x)

    image_contour2 = image_contour.copy()

    chain = []
    print(start_point)
    while True:
        # print((y, x))
        image_contour2[y, x] = 0
        subImg = np.array([image_contour2[y - 1, x - 1], image_contour2[y - 1, x], image_contour2[y - 1, x + 1],
                           image_contour2[y, x - 1],     image_contour2[y, x],     image_contour2[y, x + 1],
                           image_contour2[y + 1, x - 1], image_contour2[y + 1, x], image_contour2[y + 1, x + 1]])
        #print(subImg)

        directions = np.array([0, 1, 2,
                               7, 0, 3,
                               6, 5, 4])
        directions2 = np.array([1, 2, 3,
                                8, 0, 4,
                                7, 6, 5])
        #print(directions)
        aaa = subImg * directions2
        m = max(aaa)
        if m == 1: y -= 1; x -= 1
        elif m==2: y -= 1
        elif m==3: y -= 1; x += 1
        elif m==4: x += 1
        elif m==5: y += 1; x += 1
        elif m==6: y += 1
        elif m==7: y += 1; x -= 1
        elif m==8: x -= 1
        elif m==0: break
        if (start_point == (y, x)): break

        chain.append(m-1)

        cv2.circle(image_contour, (x-20, y-20), 1, (255, 255, 0), 1)

    print(chain)
    chain2 = chain_median(chain, 10)
    print(chain2,'\n------')

    # zredukuje po sobe jdouci strejna cisla na jedno
    # zip prochazi 2 pole zaroven; zde je tedy v x[0] hodnota prvniho pole a v x[1] druheho
    """
    alist = ['a1', 'a2', 'a3']
    blist = ['b1', 'b2', 'b3']

    for i, (a, b) in enumerate(zip(alist, blist)):
        print i, a, b
    >>> 0 a1 b1
        1 a2 b2
        2 a3 b3
    """
    chain3 = np.hstack([chain2[0], [x[0] for x in zip(chain2[1:], chain2[:-1]) if x[0]!=x[1]]])

    # print(np.hstack([chain[0], [x[0] for x in zip(chain[1:], chain[:-1]) if x[0]!=x[1]]]))
    print(chain3)


    return chain
# ================================================================================================================
if __name__ == '__main__':
    #import mahotas
    bin_image, foundObject, foundObject_border, stranaA, stranaB = black_or_b(current_frame_gray, previous_frame_gray) #.astype(np.uint8)
    #foundObject = cv2.cvtColor(foundObject, cv2.COLOR_BGR2RGB)
    # cv2.imshow('foundObject',foundObject)
    #qqq = cv2.imread('HueScale.png')
    gray = foundObjectToGray(foundObject)
    hue = foundObject_hue(foundObject)
    bbb = "{0} = Hue".format(round(hue))
    ccc = "{0} = Gray".format(gray)
    print (bbb)
    print (ccc)
    # Write some Text
    #cv2.putText(foundObject_border, bbb,(0,10), cv2.FONT_HERSHEY_SIMPLEX, 0.3,(255,255,255),1)
    #cv2.putText(foundObject_border, ccc, (0,20), cv2.FONT_HERSHEY_SIMPLEX, 0.3,(255,255,255),1)

    # nacte soubor se zapsanymi rozsahy barev a nazvy a kazdy pixel porovna a prislusnou barvu inkreementuje
    # www = getTextColorFromRGB(foundObject)
    # print(www)

    # txt = "{}x{} - brick".format(round(stranaA), round(stranaB))
    # txt2 = "{} - color".format(www)
    # cv2.putText(foundObject_border, txt, (0, 13), cv2.FONT_HERSHEY_SIMPLEX, 0.35, (255, 255, 255), 1)
    # cv2.putText(foundObject_border, txt2, (0, 25), cv2.FONT_HERSHEY_SIMPLEX, 0.35, (255, 255, 255), 1)

    ts = int(time.time())
    immmm = Image.fromarray(foundObject_border)
    immmm.save("images\\objectFound{}.bmp".format(ts))
    #cv2.imshow('images\\aaaaaaaaaaaaaaaa', foundObject)


    chain_code(bin_image)





    # import colorsys
    # r, g, b = 0, 0, 255
    # h, l, s = colorsys.rgb_to_hls(r, g, b)
    # r, g, b = colorsys.hls_to_rgb(h, l, s)
    # h, l, s = rgb2hsv(r, g, b)
    # print (r, g, b)
    # print (h, l, s)


    # print(img_gray[:,:,0].mean())
    # print(img_gray.mean())
    # print(img_gray.max(axis=0))
    # print(np.argwhere(img_gray == img_gray.max()))
    # print(img_gray.argmax())



    plt.show()