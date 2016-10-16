import numpy as np
import cv2
import matplotlib.pyplot as plt
from PIL import Image
#from PIL import ImageChop

# im1 = cv2.imread('images\\background.bmp')
# im2 = cv2.imread('images\\currentImage.bmp')

im1 = cv2.imread('images\\imageOfBackground.bmp')
im2 = cv2.imread('images\\imageOfPadWithObject.bmp')

current_frame_gray = cv2.cvtColor(im1, cv2.COLOR_BGR2RGB)
previous_frame_gray = cv2.cvtColor(im2, cv2.COLOR_BGR2RGB)

frame_diff = cv2.absdiff(im1, im2)

#cv2.imshow('frame diff ', frame_diff)

def black_or_b(im1, im2):
    diff = cv2.subtract(im1, im2)
    # threshold(diff, diff, 30, 255, CV_THRESH_BINARY);
    #
    # cv2.Dilate(diff, diff, None, 18)  # to get object blobs
    # cv2.Erode(diff, diff, None, 10)

    plt.subplot(2,2,1)
    plt.imshow(im1)
    plt.axis("off")
    plt.subplot(2,2,2)
    plt.imshow(im2)
    plt.axis("off")
    plt.subplot(2,2,3)
    plt.imshow(diff)
    plt.axis("off")



    #img555 = cv2.medianBlur(diff, 5)
    img555 = cv2.cvtColor(diff, cv2.COLOR_RGB2GRAY)
    #
    # h = histogram(diff)
    # plt.plot(h, '-r')
    # plt.figure()
    kernel = np.ones((5, 5), np.uint8)
    ret,th3 = cv2.threshold(img555,50,255,cv2.THRESH_BINARY)
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

    plt.subplot(2, 2, 4)
    plt.imshow(th3, 'gray')
    plt.axis("off")

    mask_inv = cv2.bitwise_not(th3)

    rows, cols, channels = im2.shape
    roi = im2[0:rows, 0:cols]

    # Now black-out the area of logo in ROI
    img1_bg = cv2.bitwise_and(roi, roi, mask =mask_inv)
    #cv2.imshow('img1_bg',img1_bg)

    # Take only region of logo from logo image.
    img2_fg = cv2.bitwise_and(im2, im2, mask=th3)
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
    cnt = contours[0]
    cnt = largest_areas[-1]
    area = cv2.contourArea(cnt)
    print(area)

    x, y, width, height = cv2.boundingRect(cnt)
    roi = img2_fg[y:y + height, x:x + width]
    #cv2.imshow('roi', roi)
    #cv2.imwrite("roi.png", roi)

    rect = cv2.minAreaRect(cnt)
    box = cv2.boxPoints(rect)
    box = np.int0(box)
    cv2.drawContours(im2, [box], 0, (0, 0, 255), 2)

    M = cv2.moments(cnt)
    print(M)
    cv2.drawContours(im2, [cnt], 0, (0, 255, 0), 3)
    #cv2.imshow('output', im2)
    plt.savefig('images\\saveFig.png')

    return roi

#import mahotas
c = black_or_b(current_frame_gray, previous_frame_gray).astype(np.uint8)
immmm = Image.fromarray(cv2.cvtColor(c, cv2.COLOR_BGR2RGB))
immmm.save("images\\objectFound.bmp")
#cv2.imshow('images\\aaaaaaaaaaaaaaaa', c)

plt.show()